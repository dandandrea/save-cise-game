using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class Enemy : Actor
    {
        enum directions
        {
            EAST, 
            NORTHEAST, 
            NORTH, 
            NORTHWEST, 
            WEST, 
            SOUTHWEST, 
            SOUTH, 
            SOUTHEAST,
            NUM_DIRECTIONS
        }
        protected float speed;
        protected int strength;
        protected int damageDealt;
        protected int enthusiasmBonus;
        protected float trueX;
        protected float trueY;
        protected GridCell target;
        Stack<GridCell> myPath;
        Grid aStarGrid;
        bool atCiseBuilding = false;
        bool returning = false;
        float frame = 0;
        float frameSpeed = 0.1f;
        int framesPerDirection;
        directions bearing;

       /*public Enemy( Sprite sprite ) : base(sprite)
        {
            framesPerDirection = sprite.getSubimageCount()/(int)directions.NUM_DIRECTIONS;
        }*/

        public Enemy(Sprite sprite, Grid aStarGrid, float speed, int strength, int damageDealt, int enthusiasmBonus)
            : base(sprite)
        {
            this.aStarGrid = aStarGrid;
            this.speed = speed;
            this.strength = strength;
            this.damageDealt = damageDealt;
            this.enthusiasmBonus = enthusiasmBonus;
            framesPerDirection = sprite.getSubimageCount() / (int)directions.NUM_DIRECTIONS;
            this.width /= 2;
            this.height /= 2;
            setLocation(-10, -10); // Default spawn location
        }

        public override void setLocation(int x, int y)
        {
            this.trueX = x;
            this.trueY = y;
            base.setLocation(x, y);
        }

        public override void update()
        {
            doMovement();
            doAnimation();
            this.verticalDepth = -y;
            this.horizontalDepth = -x;


            base.update();
        }

        private void doMovement()
        {
            if (checkAlive())
            {
                doLivingMovement();
            }
            else
            {
                doDeadMovement();
            }
        }

        private void doDeadMovement()
        {
            if (!returning)
            {
                returning = true;
                updatePath();
            }
            moveToNextNode();
        }

        private void doLivingMovement()
        {
            if (atCiseBuilding)
            {
                attackCise();
            }
            else
            {
                moveToNextNode();
            }
        }

        private void moveToNextNode()
        {
            if (target != null)
            {
                // move toward the cell and then, 
                // if it has arrived...
                if (moveTowardCell(target))
                {
                    if(myPath.Count > 0)
                    {
                        target = myPath.Pop();
                    }
                    else
                    {
                        // you have arrived at the final destination
                        if (checkAlive())
                        {
                            atCiseBuilding = true;
                        }
                        else
                        {
                            //Console.WriteLine("x_x");
                            // remove me from the game, I'm dead and offscreen
                            GameController.removeEnemy(this);
                        }
                    }
                }
            }
            else
            {
                // update path sets myPath AND the new target
                updatePath();
            }
        }

        private bool checkAlive()
        {
            return strength>0;
        }

        private void doAnimation()
        {
            frame += frameSpeed;
            if (frame >= framesPerDirection)
            {
                frame -= framesPerDirection;
            }
            imageIndex = (int)bearing  + (int)frame*(int)directions.NUM_DIRECTIONS;
        }

        private void attackCise()
        {
            GameController.hurtBudget(this.damageDealt);
            if (GameController.isGameOver())
            {
                strength = 0;
            }
            //Console.WriteLine(GameController.getBudget());
        }

        public void updatePath()
        {
            if(pathIsBlocked())
                {
                if (target != null)
                {
                    if (checkAlive())
                    {
                        Stack<GridCell> temp = aStarGrid.astar(target.row, target.col, GameController.CISE_COL, GameController.CISE_ROW);
                        if (temp != null)
                        {
                            myPath = temp;
                            target = myPath.Pop();
                        }

                    }
                    else
                    {
                        myPath = aStarGrid.astar(target.row, target.col, 1, 1);
                        //target = myPath.Pop();
                    }
                }
                else
                {
                    if (checkAlive())
                    {
                        if (!atCiseBuilding)
                        {
                            myPath = aStarGrid.astar(1, 1, GameController.CISE_COL, GameController.CISE_ROW);
                            target = myPath.Pop();
                        }
                    }
                    else
                    {
                        myPath = aStarGrid.astar(GameController.CISE_COL, GameController.CISE_ROW, 1, 1);
                        target = myPath.Pop();
                    }
                }
            }
        }

        private bool pathIsBlocked()
        {
            GridCell[] pathOfCells;
            if (myPath != null)
            {
                pathOfCells = myPath.ToArray();
                foreach (GridCell c in pathOfCells)
                {
                    if (aStarGrid.isCellBlocked(c.row, c.col))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private bool moveTowardCell( GridCell cell )
        {
            bool hasArrived;
            Point p = new Point(GameController.GRID_OFFSET_X + cell.col * GameController.CELL_WIDTH, GameController.GRID_OFFSET_Y + cell.row * GameController.CELL_HEIGHT);
            float dx = p.X - trueX;
            float dy = p.Y - trueY;
            float sqrDist = dx * dx + dy * dy;
            if (sqrDist < speed * speed)
            {
                trueX = p.X;
                trueY = p.Y;
                hasArrived = true;
            }
            else
            {
                float mag = (float)Math.Sqrt(sqrDist);
                dx /= mag;
                dy /= mag;
                float hspeed = speed * dx;
                float vspeed = speed * dy;

                trueX += hspeed;
                trueY += vspeed;

                bearing = determineBearing(dx, dy);

                hasArrived = false;
            }
            x = (int)trueX;
            y = (int)trueY;

            return hasArrived;
        }

        private directions determineBearing(float normalizedHspeed, float normalizedVspeed)
        {
            directions bearing;
            if (normalizedHspeed > 0.5 && normalizedHspeed < 0.93)//(hspeed > 0.5 && hspeed < 0.866)
            {
                if (normalizedVspeed < 0) { bearing = directions.NORTHEAST; }
                else { bearing = directions.SOUTHEAST; }
            }
            else if (normalizedHspeed < -0.5 && normalizedHspeed > -0.93)//(hspeed < -0.5 && hspeed > -0.866)
            {
                if (normalizedVspeed < 0) { bearing = directions.NORTHWEST; }
                else { bearing = directions.SOUTHWEST; }
            }
            else if (Math.Abs(normalizedHspeed) > Math.Abs(normalizedVspeed))
            {
                if (normalizedHspeed > 0) { bearing = directions.EAST; }
                else { bearing = directions.WEST; }
            }
            else if (normalizedVspeed > 0)
            {
                bearing = directions.SOUTH;
            }
            else
            {
                bearing = directions.NORTH;
            }
            return bearing;
        }
    }
}
