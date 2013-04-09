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
        public const int CISE_COL = 18;
        public const int CISE_ROW = 18;
        private const int NUM_LEVELS = 20; // Total number of waves 
        private const int WAVE_ALL_SPAWN_SECS = 45; // Number of seconds to spawn the complete wave in
        private static int budget = 20000000;
        private static Scene gameScene;
        private static Grid grid;
        private static List<Enemy> enemies;
        private static List<Enemy> deadEnemies;
        private static bool isGameStarted; // Whether or not gameplay has started
        private static bool isPaused = false; // Whether or not gameplay is currently paused, hardcoded to false for now
        private static List<List<Enemy>> waves; // The enemies that make up each wave
        private static int currentWaveIndex = 0; // Current wave index
        private static int currentWaveSize; // Size of the current wave
        private static double nextSpawnTime = 0.0d; // Initialize this to zero so that the first enemy spawns immediately
        private static TowerPlacer towerPlacer;

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
            gameScene.setBackground(new Sprite(ContentStore.getTexture("bg_gameArea")));



            grid = new Grid();

            generateWaves(); // Generate the waves
            currentWaveSize = waves[0].Count; // Initialize current wave size
            enemies = new List<Enemy>();
            deadEnemies = new List<Enemy>();

            towerPlacer = new TowerPlacer(new Sprite(ContentStore.getTexture("spr_whitePixel"),CELL_WIDTH,CELL_HEIGHT,1,1), 100, 100);
            gameScene.add(towerPlacer);
#if DEBUG
            GridDrawer gd = new GridDrawer();
            gameScene.add(gd);
#endif

            // buttons/side panel

            // default towers
            Button tower1 = new Button(650, 80, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            Button tower2 = new Button(700, 80, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            Button tower3 = new Button(750, 80, new Sprite(ContentStore.getTexture("spr_blockTower"), 30, 30, 1, 1));
            gameScene.add(tower1);
            gameScene.add(tower2);
            gameScene.add(tower3);
            tower1.setMouseReleasedAction(new PlaceWallTowerGameAction());
            tower2.setMouseReleasedAction(new PlaceWallTowerGameAction());
                    tower2.setActive(false);// just to demonstrate
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
            hero3.setMouseReleasedAction(new PlaceWallTowerGameAction());            return gameScene;
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
            //GameController.beginPlacingTower(towerTypes.BLOCK);
            foreach( Enemy e in deadEnemies )
            {
                enemies.Remove(e);
                gameScene.remove(e);
            }
            deadEnemies.Clear();

            // Spawn another enemy?  Only perform this check if gameplay has started and
            // there are still enemies remaining to be spawned
            if (isGameStarted == true && isPaused == false && currentWaveIndex != -1)
            {
                // Is it time to spawn another enemy?
                if (gameTime.TotalGameTime.TotalMilliseconds >= nextSpawnTime)
                {
                    #if DEBUG
                    Console.WriteLine("Spawning next enemy");
                    Console.WriteLine("totalGameTime is " + (gameTime.TotalGameTime.TotalMilliseconds / 1000) + " secs");
                    Console.WriteLine("nextSpawnTime is " + (nextSpawnTime / 1000) + " secs");
                    Console.WriteLine("Current wave size is " + currentWaveSize);
                    #endif

                    // Place the enemy and remove from waves list
                    addEnemy(waves[currentWaveIndex][0]);
                    waves[currentWaveIndex].RemoveAt(0);

                    // End of current wave?
                    if (waves[currentWaveIndex].Count == 0)
                    {
                        #if DEBUG
                        Console.WriteLine("Wave finished");
                        #endif

                        // No more waves?
                        if (currentWaveIndex == waves.Count - 1)
                        {
                            #if DEBUG
                            Console.WriteLine("No more waves");
                            #endif

                            // Set current wave index to -1
                            currentWaveIndex = -1;
                        }
                        else
                        {
                            #if DEBUG
                            Console.WriteLine("Next wave");
                            #endif

                            // There is another wave
                            currentWaveIndex++;

                            // Update current wave size
                            currentWaveSize = waves[currentWaveIndex].Count;
                        }
                    }

                    // If there are still more enemies to spawn
                    if (currentWaveIndex != -1)
                    {
                        // Calculate the next spawn time
                        nextSpawnTime = gameTime.TotalGameTime.TotalMilliseconds + (WAVE_ALL_SPAWN_SECS * 1000 / currentWaveSize);

                        #if DEBUG
                        Console.WriteLine("nextSpawnTime updated to " + (nextSpawnTime / 1000) + " secs");
                        #endif
                    }
                }
            }
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

        private static void generateWaves()
        {
            #if DEBUG
            Console.WriteLine("generateWaves() starting");
            #endif

            // Initialize the waves list
            waves = new List<List<Enemy>>();

            Sprite enemySprite = new Sprite(ContentStore.getTexture("spr_EnemyWalking"), 64, 64, 64, 8);
            Enemy test;
            // Add enemies to first wave
            List<Enemy> wave1 = new List<Enemy>(5);
            wave1.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave1.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave1.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave1.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave1.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            waves.Add(wave1);

            // Add enemies to second wave
            List<Enemy> wave2 = new List<Enemy>(4);
            wave2.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave2.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave2.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave2.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            waves.Add(wave2);

            // Add enemies to third wave
            List<Enemy> wave3 = new List<Enemy>(6);
            wave3.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave3.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave3.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave3.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave3.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            wave3.Add(new Enemy(enemySprite, grid, 0.25f, 1500, 1, 100));
            waves.Add(wave3);

            #if DEBUG
            Console.WriteLine("[" + waves.Count + "]");
            Console.WriteLine("[" + waves[0].Count + "]");
            Console.WriteLine("[" + waves[1].Count + "]");
            Console.WriteLine("[" + waves[2].Count + "]");
            #endif

            #if DEBUG
            Console.WriteLine("generateWaves() ending");
            #endif
        }
    }
}
