using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SaveCISE_Game
{
    class TowerPlacer : Actor
    {
        towerTypes typeToPlace = towerTypes.NONE;
        bool shiftDown = false;

        public TowerPlacer(Sprite sprite) : base(sprite)
        {
            visible = false;
            this.panelTier = 2;
        }

        public TowerPlacer(Sprite sprite, int x, int y): base(sprite)
        {
            visible = false;
            this.panelTier = 2;
            this.x = x;
            this.y = y;
        }

        internal override void leftMousePressed(int x, int y)
        {
            if (this.typeToPlace != towerTypes.NONE)
            {
                GameController.tryToPlaceTower(typeToPlace, x, y);
                if (!shiftDown)
                {
                    visible = false;
                    typeToPlace = towerTypes.NONE;
                }
            }
        }

        internal override void mouseOver(int x, int y)
        {
            if (typeToPlace != towerTypes.NONE)
            {
                visible = true;
            }
            if ((x > GameController.CELL_WIDTH && x < GameController.CELL_WIDTH * (GameController.GRID_WIDTH + 1)) && (y > GameController.CELL_HEIGHT && y < GameController.CELL_HEIGHT * (GameController.GRID_HEIGHT + 1)))
            {
                this.x = ((x - GameController.GRID_OFFSET_X) / GameController.CELL_WIDTH) * GameController.CELL_WIDTH + GameController.GRID_OFFSET_X;
                this.y = ((y - GameController.GRID_OFFSET_Y) / GameController.CELL_HEIGHT) * GameController.CELL_HEIGHT + GameController.GRID_OFFSET_Y;
            }
            else
            {
                visible = false;
            }
        }

        internal override void mouseDragged(int x, int y)
        {
            this.x = ((x - GameController.GRID_OFFSET_X) / GameController.CELL_WIDTH) * GameController.CELL_WIDTH + GameController.GRID_OFFSET_X;
            this.y = ((y - GameController.GRID_OFFSET_Y) / GameController.CELL_HEIGHT) * GameController.CELL_HEIGHT + GameController.GRID_OFFSET_Y;
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (visible)
            {
                float alpha = 0.5f;
                Color transparent = new Color(1.0f * alpha, 1.0f * alpha, 1.0f * alpha, alpha);
                base.draw(sb, transparent);
            }
        }

        internal void setTypeToPlace(towerTypes type)
        {
            typeToPlace = type;
        }

        internal override void keyPressed(Keys key)
        {
            if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                this.shiftDown = true;
            }
        }

        internal override void keyReleased(Keys key)
        {
            if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                this.shiftDown = false;
            }
        }
    }
}
