using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class GridDrawer : Actor
    {
        public GridDrawer() : base(new Sprite(ContentStore.getTexture("Copy of spr_whitePixel"),GameController.CELL_WIDTH-2,GameController.CELL_HEIGHT-2,1,1))
        {

        }
        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            for (int i = 1; i <= GameController.GRID_WIDTH; i++)
            {
                for (int j = 1; j <= GameController.GRID_HEIGHT; j++)
                {
                    Color alpha = new Color(100,100,100,100);
                    this.sprite.draw(sb, i * GameController.CELL_WIDTH+GameController.GRID_OFFSET_X, j * GameController.CELL_HEIGHT+GameController.GRID_OFFSET_Y, 0,1,1,alpha);
                }
            }
        }
    }
}
