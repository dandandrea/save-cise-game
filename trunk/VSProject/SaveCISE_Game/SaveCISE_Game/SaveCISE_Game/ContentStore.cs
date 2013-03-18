using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace SaveCISE_Game
{
    class ContentStore
    {
        private static Dictionary<String, Texture2D> textures = new Dictionary<String, Texture2D  >();
        private static Dictionary<String, SpriteFont>   fonts = new Dictionary<String, SpriteFont >();
        private static Dictionary<String, SoundEffect> sounds = new Dictionary<String, SoundEffect>();

        public static void addTexture(String name, Texture2D texture)
        {
            textures.Add(name, texture);
        }

        public static void addFont(String name, SpriteFont font)
        {
            fonts.Add(name, font);
        }

        public static void addSound(String name, SoundEffect sound)
        {
            sounds.Add(name, sound);
        }

        public static Texture2D getTexture(String name)
        {
            return textures[name];
        }

        public static SpriteFont getFont(String name)
        {
            return fonts[name];
        }

        public static SoundEffect getSound(String name)
        {
            return sounds[name];
        }
    }
}
