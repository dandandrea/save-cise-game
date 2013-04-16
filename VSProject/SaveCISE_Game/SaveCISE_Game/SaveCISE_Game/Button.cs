using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SaveCISE_Game
{
    class Button : Actor
    {
        private GameAction overAction = null;
        private GameAction pressedAction = null;
        private GameAction releasedAction = null;
        protected bool isActive = true;
        private bool isPressed;

        public Button(int x, int y, Sprite sprite) : base(sprite, x, y, sprite.getWidth(), sprite.getHeight())
        {
            this.x = x;
            this.y = y;
            this.sprite = sprite;
            this.width = sprite.getWidth();
            this.height = sprite.getHeight();
        }

        internal override void setSprite(Sprite sprite)
        {
            base.setSprite(sprite);
            this.width = sprite.getWidth();
            this.height = sprite.getHeight();
        }

        #region action setters
        public void setMouseOverAction(GameAction action)
        {
            overAction = action;
        }

        public void setMousePressedAction(GameAction action)
        {
            pressedAction = action;
        }

        public void setMouseReleasedAction(GameAction action)
        {
            releasedAction = action;
        }
        #endregion

        #region action methods
        private void doMouseOverAction()
        {
            if (overAction != null && isActive)
            {
                overAction.doAction();
            }
        }

        private void doMousePressedAction()
        {
            if (pressedAction != null && isActive)
            {
                pressedAction.doAction();
            }
        }

        private void doMouseReleasedAction()
        {
            if (releasedAction != null && isActive)
            {
                releasedAction.doAction();
            }
        }
        #endregion

        public override void update() { }

        public override void draw(SpriteBatch sb)
        {
            if (!isActive)
            {
                imageIndex = 3;
            }
            if (visible)
            {
                base.draw(sb);
            }
        }

        #region mouse events
        internal override void mouseOver(int x, int y)
        {
            if (visible)
            {
                if (checkCollision(x, y))
                {
                    this.imageIndex=1;
                    doMouseOverAction();
                }
                else
                {
                    this.imageIndex=0;
                }
            }
        }

        internal override void leftMousePressed(int x, int y)
        {
            if (visible)
            {
                if (checkCollision(x, y))
                {
                    this.isPressed = true;
                    this.imageIndex = 2;
                    doMousePressedAction();
                }
                else
                {
                    this.imageIndex = 0;
                }
            }
        }

        internal override void leftMouseReleased(int x, int y)
        {
            if (visible)
            {
                if (isPressed)
                {
                    if (checkCollision(x, y))
                    {
                        this.imageIndex = 1;
                        doMouseReleasedAction();
                    }
                    else
                    {
                        this.imageIndex = 0;
                    }
                }
            }
            isPressed = false;
        }
        #endregion

        #region keyboard events
        public void keyPressed(Keys key)
        {
            if (visible)
            {
            }
        }

        public void keyReleased(Keys key)
        {
            if (visible)
            {
            }
        }
        #endregion

        internal void setActive(bool active)
        {
            if(active && !isActive)
                imageIndex = 0;
            isActive = active;
        }
    }
}
