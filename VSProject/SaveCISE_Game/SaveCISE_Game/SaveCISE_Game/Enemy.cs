using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        protected int strength; // This is the enemy's hit points
        protected int startingStrength;
        protected int damageDealt; // This is the amount of damage that the enemy deals per attack
        protected bool hasBeenSlowedDown = false;
        protected int enthusiasmBonus;
        protected float trueX;
        protected float trueY;
        protected GridCell attack;
        protected GridCell target;
        Stack<GridCell> myPath;
        Grid aStarGrid;
        bool atCiseBuilding = false;
        bool returning = false;
        float frame = 0;
        float frameSpeed = 0.1f;
        int framesPerDirection;
        directions bearing;
        private Color myColor = Color.White;

       /*public Enemy( Sprite sprite ) : base(sprite)
        {
            framesPerDirection = sprite.getSubimageCount()/(int)directions.NUM_DIRECTIONS;
        }*/

        public Enemy(Sprite sprite, Grid aStarGrid, float speed, int strength, int damageDealt, int enthusiasmBonus)
            : base(sprite)
        {
            this.aStarGrid = aStarGrid;
            this.attack = aStarGrid.getRandomAttackPoint();
            //Console.WriteLine("asd" + att.col + " " + att.row);
            this.speed = speed;
            this.strength = strength;
            this.damageDealt = damageDealt;
            this.enthusiasmBonus = enthusiasmBonus;
            framesPerDirection = sprite.getSubimageCount() / (int)directions.NUM_DIRECTIONS;
            setLocation(-10, -10); // Default spawn location
            this.setOrigin(20, 40); // for drawing the sprite properly
            this.startingStrength = strength;
        }

        public override void setLocation(int x, int y)
        {
            this.trueX = x;
            this.trueY = y;
            base.setLocation(x, y);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {

            //Healthbars
            if (this.checkAlive())
            {
                double scaleX = ((double)width / sprite.getWidth());
                double scaleY = ((double)height / sprite.getHeight());
                int nextHeight = 4;
                int nextWidth = 30;
                float str = ((float)strength / (float)startingStrength) * nextWidth;
                int healthWidth = (int)str;
                Sprite healthBar = new Sprite(ContentStore.getTexture("spr_whitePixel"), nextWidth, nextHeight, 1, 1);
                Sprite healthLeft = new Sprite(ContentStore.getTexture("spr_whitePixel"), healthWidth, nextHeight, 1, 1);
                healthBar.draw(sb, (x - (int)(originX * scaleX)) + 15, (y - (int)(originY * scaleY)) + 3, imageIndex, scaleX, scaleY, Color.Red);
                healthLeft.draw(sb, (x - (int)(originX * scaleX)) + 15, (y - (int)(originY * scaleY)) + 3, imageIndex, scaleX, scaleY, Color.Green);
            }
            base.draw(sb, myColor);
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
            if (!checkAlive())
            {
                if (myColor == Color.White)
                {
                    float alpha = 0.4f;
                    Vector3 components = myColor.ToVector3();
                    myColor = new Color(components.X * alpha, components.Y * alpha, components.Z * alpha, alpha);
                }
            }
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
            if(pathIsBlocked() || !checkAlive())
                {
                if (target != null)
                {
                    if (checkAlive())
                    {
                        Stack<GridCell> temp = aStarGrid.astar(target.row, target.col, attack.row, attack.col);
                        if (temp != null)
                        {
                            myPath = temp;
                            target = myPath.Pop();
                        }

                    }
                    else
                    {
                        myPath = aStarGrid.astar(target.row, target.col, 1, 1);
                        target = myPath.Pop();
                    }
                }
                else
                {
                    if (checkAlive())
                    {
                        if (!atCiseBuilding)
                        {
                            myPath = aStarGrid.astar(1, 1, attack.row, attack.col);
                            target = myPath.Pop();
                        }
                    }
                    else
                    {
                        myPath = aStarGrid.astar(12, 12, attack.row, attack.col);
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

        public void dealDamage(int damageAmount)
        {
            this.strength = this.strength - damageAmount;
        }

        public void slowDown(float percentSlowDown)
        {
            #if DEBUG
            Console.WriteLine("[Enemy.slowDown()] Current speed is " + this.speed + ", slowing down by " + percentSlowDown + "%");
            #endif

            // Perform the slow down
            this.speed = this.speed * ((100 - percentSlowDown) / 100);
            #if DEBUG
            Console.WriteLine("[Enemy.slowDown()] New speed is " + this.speed);
            #endif
        }

        public void speedUp(float percentSlowDown)
        {
            #if DEBUG
            Console.WriteLine("[Enemy.speedUp()] Current speed is " + this.speed + ", speeding up by " + percentSlowDown + "%");
            #endif

            // Perform the speed up
            this.speed = this.speed / ((100 - percentSlowDown) / 100);

            #if DEBUG
            Console.WriteLine("[Enemy.speedUp()] New speed is " + this.speed);
            #endif
        }

        public int getStrength()
        {
            return this.strength;
        }

        public bool getHasBeenSlowedDown()
        {
            return this.hasBeenSlowedDown;
        }

        public void setHasBeenSlowedDown(bool hasBeenSlowedDown)
        {
            this.hasBeenSlowedDown = hasBeenSlowedDown;
        }
    }
}
