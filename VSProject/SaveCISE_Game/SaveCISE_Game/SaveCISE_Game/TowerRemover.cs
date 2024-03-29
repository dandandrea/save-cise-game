﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SaveCISE_Game
{
    class TowerRemover : Actor
    {
        Boolean remove;
        bool shiftDown = false;

        public TowerRemover(Sprite sprite) : base(sprite)
        {
            this.panelTier = 2;
        }

        public TowerRemover(Sprite sprite, int x, int y): base(sprite)
        {
            this.panelTier = 2;
            this.x = x;
            this.y = y;
        }

        internal void setTrue()
        {
            remove = true;
        }

        internal override void leftMousePressed(int x, int y)
        {
            if (remove)
            {
                GameController.tryToRemoveTower(x, y);
                if (!shiftDown)
                {
                    remove = false;
                }
            }
        }

        internal override void mouseOver(int x, int y)
        {
            if (remove)
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
            if (!remove)
            {
            }
            else
            {
                Color red = new Color(220, 20, 60);
                base.draw(sb, red);
            }
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
