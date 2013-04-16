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
        public Bullet(Vector2 start, Vector2 end)
            : base(new Sprite(ContentStore.getTexture("spr_")))
        {
        }
        //moves the bullet towards the enemy
        public void moveTowardEnd()
        {
            
        }

        public override void update()
        {
            moveTowardEnd();
        }
    }
}
