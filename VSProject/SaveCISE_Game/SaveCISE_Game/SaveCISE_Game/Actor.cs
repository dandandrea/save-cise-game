using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class Actor
    {
        protected Sprite sprite;
        protected int x;
        protected int y;
        protected int width;
        protected int height;
        protected int imageIndex;
        protected bool visible;

        public Actor(Sprite sprite)
        {
            this.sprite = sprite;
            this.x = 0;
            this.y = 0;
            this.width = sprite.getWidth();
            this.height = sprite.getHeight();
        }

        public Actor(Sprite sprite, int x, int y, int width, int height)
        {
            this.sprite = sprite;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void draw(SpriteBatch sb)
        {
            sprite.draw(sb, x, y, imageIndex, ((double)width / sprite.getWidth()), ((double)height / sprite.getHeight()), Color.White);
        }

        internal void setSprite(Sprite sprite)
        {
            this.sprite = sprite;// throw new NotImplementedException();
        }
    }
}
