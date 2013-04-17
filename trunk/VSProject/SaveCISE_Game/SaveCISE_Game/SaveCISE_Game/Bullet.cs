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
        float angle;
        float dx;
        float dy;
        float speed;
        float trueX;
        float trueY;
        Vector2 end;

        public Bullet(Vector2 start, Vector2 end)
            : base(new Sprite(ContentStore.getTexture("spr_shoutBullet")))
        {
            this.setOrigin(32, 64);
            trueX = start.X;
            trueY = start.Y;
            this.x = (int)start.X;
            this.y = (int)start.Y;
            this.width = 32;
            this.height = 32;
            dx = end.X -trueX;
            dy = end.Y -trueY;
            float mag = (float)Math.Sqrt(dx * dx + dy * dy);
            dx /= mag;
            dy /= mag;
            speed = 3f;
            this.end = end;
            angle = (float)Math.Atan2(dy, dx);

        }
        //moves the bullet towards the enemy
        public void moveTowardEnd()
        {
            float d = (trueX - end.X) * (trueX - end.X) + (trueY - end.Y) * (trueY - end.Y);
            if (d < speed * speed)
            {
                GameController.removeActor(this);
            }
            else
            {
                trueX += dx * speed;
                trueY += dy * speed;
                this.x = (int)trueX;
                this.y = (int)trueY;
            }
        }

        public override void update()
        {
            moveTowardEnd();
        }

        public override void draw(SpriteBatch sb)
        {
            this.draw(sb, angle+(float)Math.PI/2.0f);
        }
    }
}
