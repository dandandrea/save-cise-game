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

        // normal
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

        // fast
        internal List<Enemy> generateMob2(int x, float spd, int str, int dD, int eB)
        {
            List<Enemy> Mobs = new List<Enemy>();
            Sprite enemySprite = new Sprite(ContentStore.getTexture("spr_EnemyWalking"), 64, 64, 64, 8);
            for (int i = 0; i < x; i++)
            {
                Mobs.Add(new Enemy(enemySprite, grid, spd, str, dD, eB));
            }
            return Mobs;
        }

        // tanky
        internal List<Enemy> generateMob3(int x, float spd, int str, int dD, int eB)
        {
            List<Enemy> Mobs = new List<Enemy>();
            Sprite enemySprite = new Sprite(ContentStore.getTexture("spr_EnemyWalking"), 64, 64, 64, 8);
            for (int i = 0; i < x; i++)
            {
                Mobs.Add(new Enemy(enemySprite, grid, spd, str, dD, eB));
            }
            return Mobs;
        }

        // Boss 1
        internal List<Enemy> generateBoss1(int x, float spd, int str, int dD, int eB)
        {
            List<Enemy> Mobs = new List<Enemy>();
            Sprite enemySprite = new Sprite(ContentStore.getTexture("spr_deanAbernaughty"));
            for (int i = 0; i < x; i++)
            {
                Mobs.Add(new Enemy(enemySprite, grid, spd, str, dD, eB));
            }
            return Mobs;
        }

        // Boss 2
        internal List<Enemy> generateBoss2(int x, float spd, int str, int dD, int eB)
        {
            List<Enemy> Mobs = new List<Enemy>();
            Sprite enemySprite = new Sprite(ContentStore.getTexture("spr_govSnot"));
            for (int i = 0; i < x; i++)
            {
                Mobs.Add(new Enemy(enemySprite, grid, spd, str, dD, eB));
            }
            return Mobs;
        }
    }
}
