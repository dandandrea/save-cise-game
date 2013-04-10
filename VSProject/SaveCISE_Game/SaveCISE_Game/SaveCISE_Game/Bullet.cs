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

        public Bullet(Sprite sprite, float rotation, float speed, int x, int y, int width, int height)
            : base(sprite, x, y, width, height)
        {
            this.speed = speed;
            this.rotation = rotation;
        }

        public bool isDead()
        {
            return age > 100;
        }

        public void kill()
        {
            this.age = 200;
        }

        public void moveTowardEnemy(Enemy enemy)
        {
            Point p = new Point(GameController.GRID_OFFSET_X + enemy.getX() * GameController.CELL_WIDTH, GameController.GRID_OFFSET_Y + enemy.getY() * GameController.CELL_HEIGHT);
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

        public void update()
        {
            age++;
        }
    }
}
