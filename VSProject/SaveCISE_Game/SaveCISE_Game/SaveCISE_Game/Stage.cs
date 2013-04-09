using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SaveCISE_Game
{
    class Stage
    {
        List<Scene> scenes;
        int currentScene;

        public Stage()
        {
            scenes = new List<Scene>();
            currentScene = 0;
        }

        public void draw(SpriteBatch sb )
        {
            scenes[currentScene].draw( sb );
        }

        internal void keyPressed(Keys key)
        {
#if DEBUG
            //Console.WriteLine("Key Pressed: " + key);
#endif
            scenes[currentScene].keyPressed(key);
        }

        internal void keyReleased(Keys key)
        {
#if DEBUG
            //Console.WriteLine("Key Released: " + key);
#endif
        }

        internal void leftMousePressed(int x, int y)
        {
#if DEBUG
            //Console.WriteLine("Mouse Pressed: (" + x + "," + y + ")");
#endif
            scenes[currentScene].leftMousePressed(x, y);
        }

        internal void leftMouseReleased(int x, int y)
        {
#if DEBUG
            //Console.WriteLine("Mouse Released: (" + x + "," + y + ")");
#endif
            scenes[currentScene].leftMouseReleased(x, y);
        }

        internal void addScene(Scene scene)
        {
            scenes.Add(scene);
        }

        internal void mouseOver(int x, int y)
        {
            scenes[currentScene].mouseOver(x,y);
        }

        internal void gotoScene(int index)
        {
            if (index == 2)
            {
                GameController.startGame();
            }
                currentScene = index;
        }

        internal void Update( GameTime gameTime )
        {
            scenes[currentScene].Update(gameTime);
        }

        internal void mouseDragged(int x, int y)
        {
            scenes[currentScene].mouseDragged(x,y);
        }
    }
}
