using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class MobFactory
    {
        private static Grid grid;

        public MobFactory(Grid g)
        {
            grid = g;
        }

        internal List<Enemy> generateMob1(int x)
        {
            List<Enemy> Mobs = new List<Enemy>();
            Sprite enemySprite = new Sprite(ContentStore.getTexture("spr_EnemyWalking"), 64, 64, 64, 8);
            for (int i = 0; i < x; i++)
            {
                Mobs.Add(new Enemy(enemySprite, grid, 0.5f, 100, 1000, 100));
            }
            return Mobs;
        }

        internal List<Enemy> generateMob1(int x, float spd, int str, int dD, int eB)
        {
            List<Enemy> Mobs = new List<Enemy>();
            Sprite enemySprite = new Sprite(ContentStore.getTexture("spr_EnemyWalking"), 64, 64, 64, 8);
            for (int i = 0; i < x; i++)
            {
                Mobs.Add(new Enemy(enemySprite, grid, spd, str, dD, eB));
            }
            return Mobs;
        }
    }
}
