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
        public static int enthusiasm = 100; //starting enthusiasm
        public const int CELL_WIDTH = 40;//30;
        public const int CELL_HEIGHT = 30;//15;
        public const int GRID_OFFSET_X = 0;
        public const int GRID_OFFSET_Y = 0;
        public const int GRID_WIDTH = 14;
        public const int GRID_HEIGHT = 14;
        public const int CISE_COL = 12;
        public const int CISE_ROW = 12;
        public const int MAX_BUDGET = 20000000;
        private const int NUM_LEVELS = 20; // Total number of waves 
        private const int WAVE_ALL_SPAWN_SECS = 30; // Number of seconds to spawn the complete wave in
        private const int WAVE_SPAWN_DELAY = 15; // Delay between waves
        private static int budget = 5000;
        private static Scene gameScene;
        private static Grid grid;
        private static List<Enemy> enemies;
        private static List<Enemy> deadEnemies;
        private static List<Tower> towers;
        private static List<Tower> deleteTowers;
        private static bool isGameStarted; // Whether or not gameplay has started
        private static bool isPaused = false; // Whether or not gameplay is currently paused, hardcoded to false for now
        private static List<List<Enemy>> waves; // The enemies that make up each wave
        private static int currentWaveIndex = 0; // Current wave index
        private static int currentWaveSize; // Size of the current wave
        private static double nextSpawnTime = 0.0d; // Initialize this to zero so that the first enemy spawns immediately (stored in milliseconds)
        private static double nextWaveTime = ((double) WAVE_ALL_SPAWN_SECS + WAVE_SPAWN_DELAY) * 1000;
        private static TowerPlacer towerPlacer;
        private static TowerRemover towerRemover;
        private static MobFactory mobFactory;
        private static Button blockButton;
        public static Button yellButton;
        public static Button slowButton;

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
            mobFactory = new MobFactory(grid);

            generateWaves(); // Generate the waves
            currentWaveSize = waves[0].Count; // Initialize current wave size
            enemies = new List<Enemy>();
            deadEnemies = new List<Enemy>();
            towers = new List<Tower>();
            deleteTowers = new List<Tower>();

            towerPlacer = new TowerPlacer(new Sprite(ContentStore.getTexture("spr_whitePixel"), CELL_WIDTH, CELL_HEIGHT, 1, 1), 100, 100);
            gameScene.add(towerPlacer);

            towerRemover = new TowerRemover(new Sprite(ContentStore.getTexture("spr_whitePixel"), CELL_WIDTH, CELL_HEIGHT, 1, 1), 100, 100);
            gameScene.add(towerRemover);

            #if DEBUG
            GridDrawer gd = new GridDrawer();
            gameScene.add(gd);
            #endif

            gameScene.add(new BudgetDrawer());

            // buttons/side panel
            WhitePanel wp = new WhitePanel();
            gameScene.add(wp);

            //Tower buttons
            GameController.blockButton = new Button(647, 30, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            GameController.yellButton = new Button(696, 30, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            GameController.slowButton = new Button(745, 30, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            Button tower4 = new Button(647, 80, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            Button tower5 = new Button(696, 80, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            Button tower6 = new Button(745, 80, new Sprite(ContentStore.getTexture("spr_towerButton"), 48, 48, 4, 2));
            gameScene.add(blockButton);
            gameScene.add(yellButton);
            gameScene.add(slowButton);
            gameScene.add(tower4);
            gameScene.add(tower5);
            gameScene.add(tower6);
            blockButton.setMouseReleasedAction(new PlaceWallTowerGameAction());
            yellButton.setMouseReleasedAction(new PlaceYellTowerGameAction());
            slowButton.setMouseReleasedAction(new PlaceSlowTowerGameAction());
            tower4.setMouseReleasedAction(new PlaceWallTowerGameAction());
            tower5.setMouseReleasedAction(new PlaceWallTowerGameAction());
            tower6.setMouseReleasedAction(new PlaceWallTowerGameAction());

            Button deleteTower = new Button(696, 130, new Sprite(ContentStore.getTexture("spr_deleteButton"), 48, 48, 4, 2));
            gameScene.add(deleteTower);
            deleteTower.setMouseReleasedAction(new DeleteTowerGameAction());

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
                        Tower newTower = new Tower(new Sprite(ContentStore.getTexture("spr_blockTower")), typeToPlace);
                        newTower.setLocation(cellX * CELL_WIDTH + GRID_OFFSET_X, cellY * CELL_HEIGHT + GRID_OFFSET_Y);
                        newTower.setOrigin(10, 34);//newTower.setOrigin(0, 12);
                        towers.Add(newTower);
                        gameScene.add(newTower);
                        foreach (Enemy e in enemies)
                        {
                            e.updatePath();
                        }

                    }

                    //Discount the cost of the tower from enthusiasm
                    GameController.enthusiasm -= Tower.getTowerCost(typeToPlace);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        internal static bool tryToRemoveTower(int x, int y)
        {
            int cellX = (x - GRID_OFFSET_X) / CELL_WIDTH;
            int cellY = (y - GRID_OFFSET_Y) / CELL_HEIGHT;

            if (cellX >= 1 && cellX <= GRID_WIDTH && cellY >= 1 && cellY <= GRID_HEIGHT)
            {
                foreach (Tower t in towers)
                {
                    int cellTX = (t.getX() - GRID_OFFSET_X) / CELL_WIDTH;
                    int cellTY = (t.getY() - GRID_OFFSET_Y) / CELL_HEIGHT;

                    if ((cellX == cellTX) && (cellY == cellTY))
                    {
                        deleteTowers.Add(t);
                        grid.clearTile(cellY, cellX);
                        #if DEBUG
                        Console.WriteLine(cellX + " " + cellY);
                        #endif
                        foreach (Enemy e in enemies)
                        {
                            e.updatePath();
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        internal static void beginPlacingTower(towerTypes type)
        {
            towerPlacer.setTypeToPlace(type);
        }

        internal static void beginRemovingTower()
        {
            towerRemover.setTrue();
        }

        internal static void removeTower(Tower t)
        {
            deleteTowers.Add(t);
        }

        internal static void Update(GameTime gameTime)
        {
            //GameController.beginPlacingTower(towerTypes.BLOCK);
            foreach (Enemy e in deadEnemies)
            {
                enemies.Remove(e);
                gameScene.remove(e);
            }
            deadEnemies.Clear();

            foreach (Tower t in deleteTowers)
            {
                towers.Remove(t);
                gameScene.remove(t);
            }
            deleteTowers.Clear();

            // Iterate each tower
            // 1. Acquire new targets as they enter into the targeting range
            // 2. Drop existing targets as they leave the targeting range
            // 3. Fire at the active targets if there are any
            // 4. Remove the target if its strengh goes to zero
            foreach (Tower t in towers)
            {
                #if DEBUG
                // Console.WriteLine("Found a tower at " + t.getX() + ", " + t.getY());
                #endif

                // If this is not a tower that deals damage or slows down then skip this entire section of code
                if (t.getDamageDealt() == 0 && t.getPercentSlowDownDealt() == 0f)
                {
                    #if DEBUG
                    // Console.WriteLine("This tower does not deal damage or slow down, skipping to next tower");
                    #endif

                    // Skip to next tower
                    continue;
                }

                // Acquire new targets
                #if DEBUG
                // Console.WriteLine("Acquiring new targets, if any");
                #endif
                t.acquireNewTargets(enemies);

                // Drop existing targets that have left the targeting range
                #if DEBUG
                // Console.WriteLine("Dropping existing targets that have left the targeting range, if any");
                #endif
                t.dropTargetsOutOfRange();

                // Does this tower have a next fire time yet?  If not then set it and skip to the next tower
                if (t.getNextFireTime() == 0.0d)
                {
                    #if DEBUG
                    // Console.WriteLine("This tower doesn't have a next fire time yet, generating it now and then skipping to next tower");
                    // Console.WriteLine("This tower doesn't have a next fire time yet, generating it now and then skipping to next tower");
                    #endif

                    // Generate the next fire time
                    t.generateNextFireTime(gameTime);

                    #if DEBUG
                    // Console.WriteLine("totalGameTime is " + (gameTime.TotalGameTime.TotalMilliseconds / 1000) + " secs");
                    // Console.WriteLine("nextFireTime is " + (t.getNextFireTime() / 1000) + " secs");
                    #endif

                    // Skip to next tower, if any
                    continue;
                }

                #if DEBUG
                // Console.WriteLine("totalGameTime is " + (gameTime.TotalGameTime.TotalMilliseconds / 1000) + " secs");
                // Console.WriteLine("nextFireTime is " + (t.getNextFireTime() / 1000) + " secs");
                #endif

                // Is it not yet time for this tower to fire?
                if (t.getNextFireTime() > gameTime.TotalGameTime.TotalMilliseconds)
                {
                    #if DEBUG
                    // Console.WriteLine("It is not yet time for this tower to fire, skipping to next tower (if any)");
                    #endif

                    // Skip to next tower, if any
                    continue;
                }

                // Fire at the active targets if there are any
                #if DEBUG
                // Console.WriteLine("Firing at active target(s), if any");
                #endif
                t.fireAtActiveTargets();

                // Are there any active targets?
                if (t.getActiveTargets() != null)
                {
                    // Generate next fire time
                    t.generateNextFireTime(gameTime);

                    #if DEBUG
                    // Console.WriteLine("Generated next fire time");
                    // Console.WriteLine("totalGameTime is " + (gameTime.TotalGameTime.TotalMilliseconds / 1000) + " secs");
                    // Console.WriteLine("nextFireTime is " + (t.getNextFireTime() / 1000) + " secs");
                    #endif
                }
            }

            // Spawn another enemy?  Only perform this check if gameplay has started and
            // there are still enemies remaining to be spawned
            if (isGameStarted == true && isPaused == false && currentWaveIndex != -1)
            {
                // Is it time to spawn another enemy?
                if (gameTime.TotalGameTime.TotalMilliseconds >= nextSpawnTime)
                {
                    #if DEBUG
                    // Console.WriteLine("Spawning next enemy");
                    // Console.WriteLine("totalGameTime is " + (gameTime.TotalGameTime.TotalMilliseconds / 1000) + " secs");
                    // Console.WriteLine("nextSpawnTime is " + (nextSpawnTime / 1000) + " secs");
                    // Console.WriteLine("Current wave size is " + currentWaveSize);
                    #endif

                    if (waves[currentWaveIndex].Count != 0)
                    {
                        // Place the enemy and remove from waves list
                        addEnemy(waves[currentWaveIndex][0]);
                        waves[currentWaveIndex].RemoveAt(0);
                    }

                    // End of current wave?
                    if (waves[currentWaveIndex].Count == 0)
                    {
                        #if DEBUG
                        // Console.WriteLine("Wave finished");
                        #endif

                        // No more waves?
                        if (currentWaveIndex == waves.Count - 1)
                        {
                            #if DEBUG
                            // Console.WriteLine("No more waves");
                            #endif

                            // Set current wave index to -1
                            currentWaveIndex = -1;
                        }
                        else
                        {
                            #if DEBUG
                            // Console.WriteLine("Next wave");
                            #endif

                            if (gameTime.TotalGameTime.TotalMilliseconds >= nextWaveTime)
                            {
                                // There is another wave
                                currentWaveIndex++;

                                // Update current wave size
                                currentWaveSize = waves[currentWaveIndex].Count;
                                nextWaveTime += (WAVE_SPAWN_DELAY + WAVE_ALL_SPAWN_SECS) * 1000;
                                nextSpawnTime += nextWaveTime;
                            }
                        }
                    }

                    // If there are still more enemies to spawn
                    if (currentWaveIndex != -1)
                    {
                        // Calculate the next spawn time
                        nextSpawnTime = gameTime.TotalGameTime.TotalMilliseconds + (WAVE_ALL_SPAWN_SECS * 1000 / currentWaveSize);

                        #if DEBUG
                        // Console.WriteLine("nextSpawnTime updated to " + (nextSpawnTime / 1000) + " secs");
                        #endif
                    }
                }
            }

            if (GameController.enthusiasm < Tower.getTowerCost(towerTypes.BLOCK))
            {
                blockButton.setActive(false);
            }
            else
            {
                //blockButton.setActive(true);
            }
            if (GameController.enthusiasm < Tower.getTowerCost(towerTypes.HARM))
            {
                yellButton.setActive(false);
            }
            else
            {
                yellButton.setActive(true);
            }
            if (GameController.enthusiasm < Tower.getTowerCost(towerTypes.SLOW))
            {
                slowButton.setActive(false);
            }
            else
            {
                slowButton.setActive(true);
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
            // Console.WriteLine("generateWaves() starting");
            #endif

            // Initialize the waves list
            waves = new List<List<Enemy>>();

            // Add enemies to first wave
            waves.Add(mobFactory.generateMob1(15, .25f, 20, 10, 50));

            // Add enemies to second wave
            waves.Add(mobFactory.generateMob1(45, .25f, 100, 10, 100));

            // Add enemies to third wave
            waves.Add(mobFactory.generateMob1(45, .25f, 100, 10, 100));

            #if DEBUG
            // Console.WriteLine("[" + waves.Count + "]");
            // Console.WriteLine("[" + waves[0].Count + "]");
            // Console.WriteLine("[" + waves[1].Count + "]");
            // Console.WriteLine("[" + waves[2].Count + "]");
            #endif

            #if DEBUG
            // Console.WriteLine("generateWaves() ending");
            #endif
        }

        public static void towerShootEnemy(Tower from, Enemy target)
        {

        }
    }
}
