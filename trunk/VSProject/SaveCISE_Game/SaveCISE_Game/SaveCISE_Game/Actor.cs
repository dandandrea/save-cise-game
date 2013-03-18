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
        Sprite sprite;
        int x;
        int y;
        int width;
        int height;
        int imageIndex;
        bool visible;

        public void draw(SpriteBatch sb)
        {
            sprite.draw(sb, x, y, imageIndex, ((double)width / sprite.getWidth()), ((double)height / sprite.getHeight()), Color.White);
        }

    }
}
