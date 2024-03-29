﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class WaveSpawner
    {
        private const int WAVE_ALL_SPAWN_SECS = 30; // Number of seconds to spawn the complete wave in
        private const int WAVE_SPAWN_DELAY = 15; // Delay between waves
        private const int INITIAL_WAVE_DELAY_SECS = 10; // Number of seconds to wait before releasing first wave
        private List<List<Enemy>> waves; // The enemies that make up each wave
        private List<int> waveStart;
        private int currentWaveIndex = 0;
        private double nextMobTime = 0;
        private MobFactory mobFactory;
        private double nextMobDelay;

        public WaveSpawner(Grid g, GameTime gameTime)
        {
            mobFactory = new MobFactory(g);
            waves = new List<List<Enemy>>();

            generateWaves();
            generateTimes(gameTime);

            nextMobTime = waveStart[0];
            nextMobDelay = WAVE_ALL_SPAWN_SECS/(double)waves[0].Count;

        }

        public void spawnEnemy(GameTime gameTime)
        {
            if (currentWaveIndex != -1)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds / 1000 > nextMobTime)
                {
                    if (waves[currentWaveIndex].Count != 0)
                    {
                        // Place the enemy and remove from waves list
                        GameController.addEnemy(waves[currentWaveIndex][0]);
                        waves[currentWaveIndex].RemoveAt(0);
                        nextMobTime = nextMobTime + nextMobDelay;
                    }

                    // End of current wave?
                    if (waves[currentWaveIndex].Count == 0)
                    {
                        // No more waves?
                        if (currentWaveIndex == waves.Count - 1)
                        {
                            // Set current wave index to -1
                            GameController.lastWave();
                            currentWaveIndex = -1;
                        }
                        else
                        {
                            if (gameTime.TotalGameTime.TotalMilliseconds / 1000 >= waveStart[currentWaveIndex + 1])
                            {
                                // There is another wave
                                currentWaveIndex++;
                                // Update current wave size
                                nextMobTime = waveStart[currentWaveIndex];
                                nextMobDelay = WAVE_ALL_SPAWN_SECS / (double)waves[currentWaveIndex].Count;
                            }
                        }
                    }
                }
            }
        }

        public int getNextWaveTime(GameTime gameTime)
        {
            if (currentWaveIndex != -1)
            {
                if (waveStart[currentWaveIndex] > (int)gameTime.TotalGameTime.TotalMilliseconds / 1000)
                {
                    return waveStart[currentWaveIndex] - (int)gameTime.TotalGameTime.TotalMilliseconds / 1000;
                }
                else
                {
                    return WAVE_ALL_SPAWN_SECS + WAVE_SPAWN_DELAY + (waveStart[currentWaveIndex] - (int)gameTime.TotalGameTime.TotalMilliseconds / 1000);
                }
            }
            else
            {
                return 0;
            }
        }

        private void generateTimes(GameTime gameTime)
        {
            waveStart = new List<int>();

            waveStart.Add((int)gameTime.TotalGameTime.TotalMilliseconds / 1000 + INITIAL_WAVE_DELAY_SECS);
            for (int i = 1; i < waves.Count(); i++)
            {
                waveStart.Add(((int)gameTime.TotalGameTime.TotalMilliseconds / 1000) + INITIAL_WAVE_DELAY_SECS + i * (WAVE_ALL_SPAWN_SECS + WAVE_SPAWN_DELAY));
            }
        }

        private void generateWaves()
        {
            // Initialize the waves list
            waves = new List<List<Enemy>>();

            // waves 1-5
            waves.Add(mobFactory.generateMob1(15, 1.5f, 20, 2500, 10));
            waves.Add(mobFactory.generateMob1(20, 1.5f, 30, 2500, 10));
            waves.Add(mobFactory.generateMob2(50, 3.8f, 11, 1000, 6));
            waves.Add(mobFactory.generateMob3(5, 1f, 350, 5000, 80));
            waves.Add(mobFactory.generateMob1(25, 1.5f, 100, 2500, 20));
            // boss 1
            waves.Add(mobFactory.generateBoss1(1, 2f, 700, 100000, 500));
            // waves 6-10
            waves.Add(mobFactory.generateMob1(30, 1.5f, 270, 2500, 20));
            waves.Add(mobFactory.generateMob3(7, 1f, 2000, 5000, 100));
            waves.Add(mobFactory.generateMob2(150, 3.8f, 50, 2500, 5));
            waves.Add(mobFactory.generateMob1(50, 1.5f, 400, 2500, 20));
            waves.Add(mobFactory.generateMob3(15, 1f, 2500, 5000, 50));
            // boss 2
            waves.Add(mobFactory.generateBoss2(1, 2f, 7000, 100000, 0));
        }
    }
}
