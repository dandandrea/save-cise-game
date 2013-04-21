using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace SaveCISE_Game
{
    enum towerTypes
    {
        BLOCK,
        SLOW,
        HARM,
        DANKEL,
        DAVIS,
        BERMUDEZ,
        NUM_TYPES,
        NONE
    }

    static class GameController
    {
        public const int CELL_WIDTH = 40;//30;
        public const int CELL_HEIGHT = 30;//15;
        public const int GRID_OFFSET_X = 0;
        public const int GRID_OFFSET_Y = 0;
        public const int GRID_WIDTH = 14;
        public const int GRID_HEIGHT = 14;
        public const int CISE_COL = 12;
        public const int CISE_ROW = 12;
        public const int MAX_BUDGET = 20000000;
        public const int INITIAL_ENTHUSIASM = 145;
        public static int nextWaveCountdown = 0;
        public static int enthusiasm = INITIAL_ENTHUSIASM; //starting enthusiasm
        public static int budget = MAX_BUDGET;
        private static Scene gameScene;
        private static Grid grid;
        private static List<Enemy> enemies;
        private static List<Enemy> deadEnemies;
        private static List<Tower> towers;
        private static List<Tower> deleteTowers;
        private static List<Actor> deleteActors;
        private static bool isGameStarted; // Whether or not gameplay has started
        private static TowerPlacer towerPlacer;
        private static TowerRemover towerRemover;
        private static Button blockButton;
        public static Button yellButton;
        public static Button slowButton;
        public static Button dankelButton;
        public static Button davisButton;
        public static Button bermudezButton;
        private static SoundEffectInstance footsteps;
        public static WaveSpawner waveSpawner;
        private static bool isLastWave = false;
        

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
            gameScene.setBackground(new Sprite(ContentStore.getTexture("Copy of bg_gameArea")));

            grid = new Grid();
            //mobFactory = new MobFactory(grid);

            //generateWaves(); // Generate the waves
            //currentWaveSize = waves[0].Count; // Initialize current wave size

            enemies = new List<Enemy>();
            deadEnemies = new List<Enemy>();
            towers = new List<Tower>();
            deleteTowers = new List<Tower>();
            deleteActors = new List<Actor>();

            towerPlacer = new TowerPlacer(new Sprite(ContentStore.getTexture("Copy of spr_whitePixel"), CELL_WIDTH, CELL_HEIGHT, 1, 1), 100, 100);
            gameScene.add(towerPlacer);

            towerRemover = new TowerRemover(new Sprite(ContentStore.getTexture("Copy of spr_whitePixel"), CELL_WIDTH, CELL_HEIGHT, 1, 1), 100, 100);
            gameScene.add(towerRemover);

            #if DEBUG
            GridDrawer gd = new GridDrawer();
            gameScene.add(gd);
            #endif

            // buttons/side panel
            WhitePanel wp = new WhitePanel();
            gameScene.add(wp);

            //Tower buttons
            GameController.blockButton = new Button(647, 30, new Sprite(ContentStore.getTexture("Copy of spr_blockButton"), 48, 48, 4, 2));
            GameController.yellButton = new Button(696, 30, new Sprite(ContentStore.getTexture("Copy of spr_yellButton"), 48, 48, 4, 2));
            GameController.slowButton = new Button(745, 30, new Sprite(ContentStore.getTexture("Copy of spr_slowButton"), 48, 48, 4, 2));
            GameController.dankelButton = new Button(647, 80, new Sprite(ContentStore.getTexture("Copy of spr_dankel"), 48, 48, 4, 2));
            GameController.davisButton = new Button(696, 80, new Sprite(ContentStore.getTexture("Copy of spr_davis"), 48, 48, 4, 2));
            GameController.bermudezButton = new Button(745, 80, new Sprite(ContentStore.getTexture("Copy of spr_bermudez"), 48, 48, 4, 2));
            gameScene.add(blockButton);
            gameScene.add(yellButton);
            gameScene.add(slowButton);
            gameScene.add(dankelButton);
            gameScene.add(davisButton);
            gameScene.add(bermudezButton);
            blockButton.setMouseOverAction(new BlockTowerDescriptionGameAction());
            yellButton.setMouseOverAction(new ShoutTowerDescriptionGameAction());
            slowButton.setMouseOverAction(new SlowTowerDescriptionGameAction());
            dankelButton.setMouseOverAction(new DankelTowerDescriptionGameAction());
            davisButton.setMouseOverAction(new DavisTowerDescriptionGameAction());
            bermudezButton.setMouseOverAction(new BermudezTowerDescriptionGameAction());
            blockButton.setMouseReleasedAction(new PlaceWallTowerGameAction());
            yellButton.setMouseReleasedAction(new PlaceYellTowerGameAction());
            slowButton.setMouseReleasedAction(new PlaceSlowTowerGameAction());
            dankelButton.setMouseReleasedAction(new PlaceDankelTowerGameAction());
            davisButton.setMouseReleasedAction(new PlaceDavisTowerGameAction());
            bermudezButton.setMouseReleasedAction(new PlaceBermudezTowerGameAction());

            Button deleteTower = new Button(696, 130, new Sprite(ContentStore.getTexture("Copy of spr_deleteButton"), 48, 48, 4, 2));
            gameScene.add(deleteTower);
            deleteTower.setMouseReleasedAction(new DeleteTowerGameAction());
            deleteTower.setMouseOverAction(new DeleteTowerDescriptionGameAction());

            gameScene.add(new BudgetDrawer());

            footsteps = ContentStore.getSound("snd_steps").CreateInstance();

            return gameScene;
        }

        internal static bool tryToPlaceTower(towerTypes typeToPlace, int x, int y)
        {
            if(Tower.getTowerCost(typeToPlace) > enthusiasm)
            {
                return false;
            }
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
                        Tower newTower = new Tower(new Sprite(ContentStore.getTexture("Copy of spr_blockTower")), typeToPlace);
                        newTower.setLocation(cellX * CELL_WIDTH + GRID_OFFSET_X, cellY * CELL_HEIGHT + GRID_OFFSET_Y);
                        newTower.setOrigin(10, 34);//newTower.setOrigin(0, 12);
                        towers.Add(newTower);
                        gameScene.add(newTower);

                        newTower.dankelDamageBoost(towers);

                        foreach (Enemy e in enemies)
                        {
                            e.updatePath();
                        }

                    }

                    //Sutract the cost of the tower from enthusiasm
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

                        //Return half cost of tower
                        float subsidy = (float)0.5 * Tower.getTowerCost(t.getTowerType());
                        GameController.enthusiasm += (int) subsidy;

                        if (t.getTowerType() == towerTypes.DANKEL)
                        {
                            t.dankelDamageBoost(towers, true);
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

        internal static void reset()
        {
            GameController.waveSpawner = null;
            GameController.budget = GameController.MAX_BUDGET;
            GameController.enthusiasm = INITIAL_ENTHUSIASM;
            GameController.buildGameScene();
        }

        internal static void Update(GameTime gameTime)
        {
            if (isGameStarted && waveSpawner == null)
            {
                waveSpawner = new WaveSpawner(grid, gameTime);
            }
            /*
            if (isGameStarted == true && nextWaveTime == 0.0d)
            {
                nextWaveTime = (((double)WAVE_ALL_SPAWN_SECS + WAVE_SPAWN_DELAY) * 1000) + gameTime.TotalGameTime.TotalMilliseconds;
                Console.WriteLine("Next wave time init'd to " + nextWaveTime + ", total game time is " + gameTime.TotalGameTime.TotalMilliseconds);

                nextSpawnTime = gameTime.TotalGameTime.TotalMilliseconds + (INITIAL_WAVE_DELAY_SECS * 1000);
                Console.WriteLine("Initial spawn time init'd to " + nextSpawnTime + ", total game time is " + gameTime.TotalGameTime.TotalMilliseconds);
            }
            */

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

            foreach (Actor a in deleteActors)
            {
                gameScene.remove(a);
            }
            deleteActors.Clear();

            if (enemies.Count > 0)
            {
                if (footsteps.State != SoundState.Playing)
                {
                    //footsteps.IsLooped = true;
                    footsteps.Play();
                }
            }
            else if (footsteps.State == SoundState.Playing)
            {
                footsteps.Pause();
            }

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
                if (t.getDamageDealt() == 0 && t.getPercentSlowDownDealt() == 0f && t.getTowerType() != towerTypes.BERMUDEZ && t.getTowerType() != towerTypes.DAVIS)
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

            //calls enemies
            if (waveSpawner != null)
            {
                waveSpawner.spawnEnemy(gameTime);
                nextWaveCountdown = waveSpawner.getNextWaveTime(gameTime);
            }

            /*

            // Spawn another enemy?  Only perform this check if gameplay has started and
            // there are still enemies remaining to be spawned
            if (isGameStarted == true && isPaused == false && currentWaveIndex != -1)
            {
                // Is it time to spawn another enemy?
                if (gameTime.TotalGameTime.TotalMilliseconds >= nextSpawnTime)
                {
                    // Mark first enemy released
                    isFirstEnemyReleased = true;

                    if (waves[currentWaveIndex].Count != 0)
                    {
                        #if DEBUG
                        // Console.WriteLine("Spawning next enemy at " + gameTime.TotalGameTime.TotalMilliseconds);
                        // Console.WriteLine("nextSpawnTime is " + (nextSpawnTime / 1000) + " secs");
                        // Console.WriteLine("Current wave size is " + currentWaveSize);
                        #endif

                        // Place the enemy and remove from waves list
                        addEnemy(waves[currentWaveIndex][0]);
                        waves[currentWaveIndex].RemoveAt(0);
                    }

                    // End of current wave?
                    if (waves[currentWaveIndex].Count == 0)
                    {
                        #if DEBUG
                        // Console.WriteLine("Wave finished at " + gameTime.TotalGameTime.TotalMilliseconds + ", next wave time is " + nextWaveTime);
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
                            if (gameTime.TotalGameTime.TotalMilliseconds >= nextWaveTime)
                            {
                                // There is another wave
                                currentWaveIndex++;

                                // Update current wave size
                                currentWaveSize = waves[currentWaveIndex].Count;
                                nextWaveTime += (WAVE_SPAWN_DELAY + WAVE_ALL_SPAWN_SECS) * 1000;
                                nextSpawnTime += nextWaveTime;

                                #if DEBUG
                                Console.WriteLine("Wave number " + (currentWaveIndex + 1));
                                // Console.WriteLine("Wave released at " + gameTime.TotalGameTime.TotalMilliseconds + ", next spawn time is " + nextSpawnTime + ", next wave time is " + nextWaveTime);
                                #endif
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
              
            */

            if (GameController.enthusiasm < Tower.getTowerCost(towerTypes.BLOCK))
            {
                blockButton.setActive(false);
            }
            else
            {
                blockButton.setActive(true);
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
            if (GameController.enthusiasm < Tower.getTowerCost(towerTypes.DANKEL))
            {
                dankelButton.setActive(false);
            }
            else
            {
                dankelButton.setActive(true);
            }
            if (GameController.enthusiasm < Tower.getTowerCost(towerTypes.DAVIS))
            {
                davisButton.setActive(false);
            }
            else
            {
                davisButton.setActive(true);
            }
            if (GameController.enthusiasm < Tower.getTowerCost(towerTypes.BERMUDEZ))
            {
                bermudezButton.setActive(false);
            }
            else
            {
                bermudezButton.setActive(true);
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

        public static void towerShootEnemy(Tower from, Enemy target)
        {
            #if DEBUG
            // Console.WriteLine("FIRE!!!!");
            #endif

            Color bulletColor = Color.White;
            if (from.damageBoost)
            {
                bulletColor = Color.Red;
            }
            else if (from.getTowerType() == towerTypes.SLOW)
            {
                bulletColor = Color.CornflowerBlue;
            }

            gameScene.add(new Bullet(new Vector2(from.getX() + from.getWidth() / 4.0f, from.getY() - from.getHeight() / 4.0f), new Vector2(target.getX() + target.getWidth() / 4.0f, target.getY() - target.getHeight() / 4.0f), bulletColor));
        }

        internal static void removeActor(Actor a)
        {
            deleteActors.Add(a);
        }

        internal static void lastWave()
        {
            isLastWave = true;
        }

        internal static bool isGameWon()
        {
            return isLastWave && (enemies.Count == 0);
        }
    }
}
