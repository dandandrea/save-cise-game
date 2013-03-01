using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class Sprite
    {
        Texture2D spriteSheet;
        int spriteWidth;
        int spriteHeight;
        int numImages;
        int numCols;

        public Sprite(Texture2D spriteSheet)
        {
            initialize(spriteSheet, spriteSheet.Bounds.Width, spriteSheet.Bounds.Height, 1, 1);
        }

        public Sprite(Texture2D spriteSheet, int width, int height, int numImages, int imagesPerRow)
        {
            initialize(spriteSheet, width, height, numImages, imagesPerRow);
        }

        private void initialize(Texture2D spriteSheet, int width, int height, int numImages, int numCols)
        {
            this.spriteSheet = spriteSheet;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.numImages = numImages;
            this.numCols = numCols;
        }

        public void draw(SpriteBatch sb, int x, int y, int imageIndex)
        {
            draw(sb, x, y, imageIndex, 1.0, 1.0, Color.White);
        }

        public void draw(SpriteBatch sb, int x, int y, int imageIndex, double scaleX, double scaleY, Color color)
        {
            imageIndex %= numImages;
            int col = imageIndex % numCols;
            int row = imageIndex / numCols;
            Rectangle src = new Rectangle(spriteWidth * col, spriteHeight * row, spriteWidth, spriteHeight);
            Rectangle dest = new Rectangle(x, y, (int)(spriteWidth * scaleX), (int)(spriteHeight * scaleY));
            sb.Begin();
            sb.Draw(spriteSheet, dest, src, color);
            sb.End();
        }

        public int getWidth()
        {
            return spriteWidth;
        }

        public int getHeight()
        {
            return spriteHeight;
        }

        public int getSubimageCount()
        {
            return numImages;
        }
    }
}
