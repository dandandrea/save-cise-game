using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
        private static TowerPlacer towerPlacer;
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

            towerPlacer = new TowerPlacer(new Sprite(ContentStore.getTexture("spr_cell")), 100, 100);
            gameScene.add(towerPlacer);

            // buttons/side panel

            // default towers
            Button tower1 = new Button(650, 80, new Sprite(ContentStore.getTexture("spr_blockTower"), 30, 30, 1, 1));
            Button tower2 = new Button(700, 80, new Sprite(ContentStore.getTexture("spr_blockTower"), 30, 30, 1, 1));
            Button tower3 = new Button(750, 80, new Sprite(ContentStore.getTexture("spr_blockTower"), 30, 30, 1, 1));
            gameScene.add(tower1);
            gameScene.add(tower2);
            gameScene.add(tower3);
            tower1.setMouseReleasedAction(new PlaceWallTowerGameAction());
            tower2.setMouseReleasedAction(new PlaceWallTowerGameAction());
            tower3.setMouseReleasedAction(new PlaceWallTowerGameAction());


            // hero towers
            Button hero1 = new Button(650, 150, new Sprite(ContentStore.getTexture("spr_blockTower"), 30, 30, 1, 1));
            Button hero2 = new Button(700, 150, new Sprite(ContentStore.getTexture("spr_blockTower"), 30, 30, 1, 1));
            Button hero3 = new Button(750, 150, new Sprite(ContentStore.getTexture("spr_blockTower"), 30, 30, 1, 1));
            gameScene.add(hero1);
            gameScene.add(hero2);
            gameScene.add(hero3);
            hero1.setMouseReleasedAction(new PlaceWallTowerGameAction());
            hero2.setMouseReleasedAction(new PlaceWallTowerGameAction());
            hero3.setMouseReleasedAction(new PlaceWallTowerGameAction());

            return gameScene;
        }

        internal static bool tryToPlaceTower(towerTypes typeToPlace, int x, int y)
        {
            if (typeToPlace != towerTypes.NONE)
            {
                int cellX = (x - GRID_OFFSET_X) / CELL_WIDTH;
                int cellY = (y - GRID_OFFSET_Y) / CELL_HEIGHT;

                if (cellX >= 1 && cellX <= GRID_WIDTH && cellY >= 1 && cellY <= GRID_HEIGHT)
                {
                    if (grid.isCellBlocked(cellY, cellX) || (cellX == CISE_COL && cellY == CISE_ROW) || (cellX == 1 & cellY == 1))
                    {
                        return false;
                    }
                    else
                    {
                        grid.markTile(cellY, cellX);
                        if (grid.astar(1, 1, CISE_ROW, CISE_COL) == null)
                        {
                            grid.clearTile(cellY, cellX);
                            return false;
                        }
                        Actor newTower = new Actor(new Sprite(ContentStore.getTexture("spr_blockTower")));
                        newTower.setLocation(cellX * CELL_WIDTH + GRID_OFFSET_X, cellY * CELL_HEIGHT + GRID_OFFSET_Y);
                        newTower.setOrigin(0, 12);
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
            return false;
        }

        internal static void beginPlacingTower(towerTypes type)
        {
            towerPlacer.setTypeToPlace(type);
        }

        internal static void Update( GameTime gameTime )
        {
            foreach( Enemy e in deadEnemies )
            {
                enemies.Remove(e);
                gameScene.remove(e);
            }
            deadEnemies.Clear();

            //PlaceWallTowerGameAction pwt = new PlaceWallTowerGameAction();
            //pwt.doAction();


            //Next wave enemies goes here??

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
