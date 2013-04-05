using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class TowerPlacer : Actor
    {
        towerTypes typeToPlace = towerTypes.NONE;

        public TowerPlacer(Sprite sprite) : base(sprite)
        {
        }

        public TowerPlacer(Sprite sprite, int x, int y): base(sprite)
        {
            this.x = x;
            this.y = y;
        }

        internal override void leftMouseReleased(int x, int y)
        {
            /*switch (typeToPlace)
            //{
                case towerTypes.NONE :
                    //Enemy oneGuy = new Enemy(new Sprite(ContentStore.getTexture("spr_enemy"), 45, 22, 16, 8), new Grid(), 1.0f, 500, 1, 100);
                    //oneGuy.setLocation(x, y);
                    //GameController.getGameScene().add(oneGuy);

                    break;

                default :
                    //nothing
                    break;
            }*/
                GameController.tryToPlaceTower(typeToPlace, x, y);
                typeToPlace = towerTypes.NONE;
        }

        internal override void mouseOver(int x, int y)
        {
            this.x = ((x - GameController.GRID_OFFSET_X) / GameController.CELL_WIDTH) * GameController.CELL_WIDTH + GameController.GRID_OFFSET_X;
            this.y = ((y - GameController.GRID_OFFSET_Y) / GameController.CELL_HEIGHT) * GameController.CELL_HEIGHT + GameController.GRID_OFFSET_Y;
        }

        internal override void mouseDragged(int x, int y)
        {
            this.x = ((x - GameController.GRID_OFFSET_X) / GameController.CELL_WIDTH) * GameController.CELL_WIDTH + GameController.GRID_OFFSET_X;
            this.y = ((y - GameController.GRID_OFFSET_Y) / GameController.CELL_HEIGHT) * GameController.CELL_HEIGHT + GameController.GRID_OFFSET_Y;
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            float alpha = 0.5f;
            Color transparent = new Color(1.0f * alpha, 1.0f * alpha, 1.0f * alpha, alpha);
            base.draw(sb, transparent);
        }

        internal void setTypeToPlace(towerTypes type)
        {
            typeToPlace = type;
        }
    }
}
