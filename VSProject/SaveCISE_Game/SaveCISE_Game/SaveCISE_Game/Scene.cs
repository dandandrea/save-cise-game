using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SaveCISE_Game
{
    class Scene
    {
        Sprite background;
        List<Actor> actors;
        List<Actor> toAdd;

        public Scene()
        {
            actors = new List<Actor>();
            toAdd = new List<Actor>();
        }

        public void setBackground(Sprite background)
        {
            this.background = background;
        }

        public void add(Actor actor)
        {
            this.toAdd.Add(actor);
        }

        public void draw( SpriteBatch sb )
        {
            if (background != null)
            {
                background.draw(sb, 0, 0, 0);
            }
            foreach (Actor a in actors)
            {
                a.draw(sb);
            }
        }

        internal void mouseOver(int x, int y)
        {
            foreach (Actor a in actors)
            {
                a.mouseOver(x, y);
            }
        }

        internal void leftMousePressed(int x, int y)
        {
            foreach (Actor a in actors)
            {
                a.leftMousePressed(x, y);
            }
        }

        internal void leftMouseReleased(int x, int y)
        {
            foreach (Actor a in actors)
            {
                a.leftMouseReleased(x, y);
            }
        }

        internal void update()
        {
            GameController.update();
            foreach (Actor a in toAdd)
            {
                actors.Add(a);
            }
            actors.Sort(Actor.getComparer());
            toAdd.Clear();
            foreach (Actor a in actors)
            {
                a.update();
            }
        }

        internal void remove(Actor a)
        {
            actors.Remove(a);
        }

        internal void mouseDragged(int x, int y)
        {
            foreach (Actor a in actors)
            {
                a.mouseDragged(x,y);
            }
        }
    }
}
