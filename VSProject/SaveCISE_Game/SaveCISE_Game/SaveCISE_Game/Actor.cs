﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class ActorComparer : Comparer<Actor>
    {

        public override int Compare(Actor a1, Actor a2)
        {
            /*if (x.getDepth() > y.getDepth())
            {
                return -1;
            }
            else if (x.getDepth() < y.getDepth())
            {
                return 1;
            }
            else
            {
                return 0;
            }*/
            if (a1.getPanelTier()!= 0 || a2.getPanelTier()!=0)
            {
                return a2.getPanelTier() - a1.getPanelTier();
            }
            else if (a2.getVerticalDepth() == a1.getVerticalDepth())
            {
                return a2.getHorizontalDepth() - a1.getHorizontalDepth();
            }

            return a2.getVerticalDepth() - a1.getVerticalDepth();
        }
    }
    class Actor
    {
        protected Sprite sprite;
        protected int x;
        protected int y;
        protected int width;
        protected int height;
        protected int imageIndex;
        protected bool visible;
        protected int originX = 0;
        protected int originY = 0;
        protected int verticalDepth = 0;
        protected int horizontalDepth = 0;
        protected int panelTier = 0;

        public Actor(Sprite sprite)
        {
            this.sprite = sprite;
            this.x = 0;
            this.y = 0;
            this.width = sprite.getWidth();
            this.height = sprite.getHeight();
            this.visible = true;
        }

        public Actor(Sprite sprite, int x, int y, int width, int height)
        {
            this.sprite = sprite;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.visible = true;
        }

        public int getPanelTier() 
        {
            return this.panelTier;
        }

        public virtual void setLocation(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.verticalDepth = -y;
            this.horizontalDepth = -x;
        }

        public virtual void draw(SpriteBatch sb)
        {
            double scaleX = ((double)width / sprite.getWidth());
            double scaleY = ((double)height / sprite.getHeight());
            sprite.draw(sb, x - (int)(originX*scaleX), y - (int)(originY*scaleY), imageIndex, scaleX, scaleY, Color.White);

#if DEBUG
            Sprite originDrawer = new Sprite(ContentStore.getTexture("Copy of spr_whitePixel"));
            originDrawer.draw(sb, x-1, y-1, 0, 3, 3, Color.Red);
#endif
        }

        public virtual void draw(SpriteBatch sb, Color color)
        {
            double scaleX = ((double)width / sprite.getWidth());
            double scaleY = ((double)height / sprite.getHeight());
            sprite.draw(sb, x - (int)(originX * scaleX), y - (int)(originY * scaleY), imageIndex, scaleX, scaleY, color);
            
#if DEBUG
            Sprite originDrawer = new Sprite(ContentStore.getTexture("Copy of spr_whitePixel"));
            originDrawer.draw(sb, x-1, y-1,0,3,3,Color.Red);
#endif
        }

        public virtual void draw(SpriteBatch sb, float angle)
        {
            double scaleX = ((double)width / sprite.getWidth());
            double scaleY = ((double)height / sprite.getHeight());
            sprite.drawAngled(sb, x, y, angle, originX, originY, scaleX, scaleY);

#if DEBUG
            Sprite originDrawer = new Sprite(ContentStore.getTexture("Copy of spr_whitePixel"));
            originDrawer.draw(sb, x-1, y-1,0,3,3,Color.Red);
#endif
        }

        public virtual void draw(SpriteBatch sb, float angle, Color color)
        {
            double scaleX = ((double)width / sprite.getWidth());
            double scaleY = ((double)height / sprite.getHeight());
            sprite.drawAngled(sb, x, y, angle, originX, originY, scaleX, scaleY, color);

#if DEBUG
            Sprite originDrawer = new Sprite(ContentStore.getTexture("Copy of spr_whitePixel"));
            originDrawer.draw(sb, x - 1, y - 1, 0, 3, 3, Color.Red);
#endif
        }

        public virtual void update()
        {
            
        }

        public void setOrigin(int x, int y)
        {
            this.originX = x;
            this.originY = y;
        }

        public int getVerticalDepth()
        {
            return this.verticalDepth;
        }

        public int getHorizontalDepth()
        {
            return this.horizontalDepth;
        }

        public int getX()
        {
            return this.x;
        }

        public int getY()
        {
            return this.y;
        }

        internal virtual void setSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public virtual bool checkCollision(int x, int y)
        {
            return (x >= this.x && y >= this.y && x < this.x + this.width && y < this.y + this.height);
        }

        internal virtual void mouseOver(int x, int y)
        {
            //throw new NotImplementedException();
        }

        internal virtual void leftMousePressed(int x, int y)
        {
            //throw new NotImplementedException();
        }

        internal virtual void leftMouseReleased(int x, int y)
        {
            //throw new NotImplementedException();
        }

        internal virtual bool isRemoved()
        {
            return false; ;
        }

        private static ActorComparer comparer;
        public static Comparer<Actor> getComparer()
        {
            if(comparer == null)
            {
                comparer = new ActorComparer();
            }
            return comparer;
        }

        internal virtual void mouseDragged(int x, int y)
        {
            
        }

        internal void scale(double scale)
        {
            this.width = (int)(width*scale);
            this.height = (int)(height * scale);
        }

        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return height;
        }

        internal virtual void keyPressed(Microsoft.Xna.Framework.Input.Keys key)
        {
            //throw new NotImplementedException();
        }

        internal virtual void keyReleased(Microsoft.Xna.Framework.Input.Keys key)
        {
            //throw new NotImplementedException();
        }
    }
}
