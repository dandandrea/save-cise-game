using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class Bullet : Actor
    {
        private int age;
        private float speed;
        private float rotation;
        protected float trueX;
        protected float trueY;
        private Enemy target;

        public Bullet(Sprite sprite, float rotation, float speed, Enemy target, int x, int y, int width, int height)
            : base(sprite, x, y, width, height)
        {
            this.speed = speed;
            this.rotation = rotation;
        }

        // checks to see if the bullet is dead
        public bool isDead()
        {
            return age > 100;
        }

        // kills the bullet when it hits something or goes off the screen
        public void kill()
        {
            this.age = 200;
        }

        //moves the bullet towards the enemy
        public void moveTowardEnemy()
        {
            Point p = new Point(GameController.GRID_OFFSET_X + target.getX() * GameController.CELL_WIDTH, GameController.GRID_OFFSET_Y + target.getY() * GameController.CELL_HEIGHT);
            float dx = p.X - trueX;
            float dy = p.Y - trueY;
            float sqrDist = dx * dx + dy * dy;
            if (sqrDist < speed * speed)
            {
                trueX = p.X;
                trueY = p.Y;
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

            }
            x = (int)trueX;
            y = (int)trueY;
        }

        public override void update()
        {
            age++;
            moveTowardEnemy();
        }
    }
}
