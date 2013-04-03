using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    enum towerTypes
    {
        BLOCK,
        SLOW,
        HARM,

        NUM_TYPES,
        NONE
    }

    static class GameController
    {
        public const int CELL_WIDTH = 25;//30;
        public const int CELL_HEIGHT = 18;//15;
        public const int GRID_OFFSET_X = 0;
        public const int GRID_OFFSET_Y = 0;
        public const int GRID_WIDTH = 25;
        public const int GRID_HEIGHT = 25;
        public const int CISE_COL = 25;
        public const int CISE_ROW = 25;
        private static int budget = 20000000;
        private static Scene gameScene;
        private static Grid grid;
        private static List<Enemy> enemies;
        private static List<Enemy> deadEnemies;
        private static bool isGameStarted;

        public static void hurtBudget(int damage)
        {
            budget -= damage;

            if (budget < 0)
            {
                budget = 0;
            }
        }

        public static bool isGameOver()
        {
            return (budget <= 0);
        }

        public static int getBudget()
        {
            return budget;
        }

        public static Scene getGameScene()
        {
            if (gameScene == null)
            {
                return buildGameScene();
            }
            else
            {
                return gameScene;
            }
        }

        private static Scene buildGameScene()
        {
            // Build Play Scene Here
            gameScene = new Scene();
            grid = new Grid();
            
            enemies = new List<Enemy>();
            deadEnemies = new List<Enemy>();
            Enemy oneGuy = new Enemy(new Sprite(ContentStore.getTexture("spr_enemy"), 45, 22, 16, 8), grid, 0.25f, 500, 1, 100);
            oneGuy.setLocation(-50, -50);
            addEnemy(oneGuy);
            oneGuy = new Enemy(new Sprite(ContentStore.getTexture("spr_enemy"), 45, 22, 16, 8), grid, 0.25f, 1500, 1, 100);
            oneGuy.setLocation(-80, -70);
            addEnemy(oneGuy);
            oneGuy = new Enemy(new Sprite(ContentStore.getTexture("spr_enemy"), 45, 22, 16, 8), grid, 0.25f, 1500, 1, 100);
            oneGuy.setLocation(-100, -80);
            addEnemy(oneGuy);
            oneGuy = new Enemy(new Sprite(ContentStore.getTexture("spr_enemy"), 45, 22, 16, 8), grid, 0.25f, 1500, 1, 100);
            oneGuy.setLocation(-20, -20);
            addEnemy(oneGuy);

            TowerPlacer b = new TowerPlacer(new Sprite(ContentStore.getTexture("spr_cell")), 100, 100);
            gameScene.add(b);
            return gameScene;
        }

        internal static bool tryToPlaceTower(towerTypes typeToPlace, int x, int y)
        {
            int cellX = (x-GRID_OFFSET_X) / CELL_WIDTH;
            int cellY = (y-GRID_OFFSET_Y) / CELL_HEIGHT;

            if (cellX >= 1 && cellX <= GRID_WIDTH && cellY >= 1 && cellY <= GRID_HEIGHT)
            {
                if (grid.isCellBlocked(cellY, cellX) || (cellX == CISE_COL && cellY == CISE_ROW) || (cellX == 1 & cellY == 1))
                {
                    return false;
                }
                else
                {
                    grid.markTile(cellY, cellX);
                    Actor newTower = new Actor(new Sprite(ContentStore.getTexture("spr_blockTower")));
                    newTower.setLocation(cellX * CELL_WIDTH + GRID_OFFSET_X, cellY * CELL_HEIGHT + GRID_OFFSET_Y);
                    newTower.setOrigin(0,12);
                    gameScene.add(newTower);
                    foreach (Enemy e in enemies)
                    {
                        e.updatePath();
                    }

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void update()
        {
            foreach( Enemy e in deadEnemies )
            {
                enemies.Remove(e);
                gameScene.remove(e);
            }
            deadEnemies.Clear();
        }

        internal static void removeEnemy(Enemy e)
        {
            deadEnemies.Add(e);
        }

        internal static void addEnemy(Enemy e)
        {
            enemies.Add(e);
            gameScene.add(e);
        }

        internal static void startGame()
        {
            isGameStarted = true;
        }
    }
}
