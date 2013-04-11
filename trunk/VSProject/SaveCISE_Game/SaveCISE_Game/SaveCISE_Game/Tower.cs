using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class Tower : Actor
    {
        private towerTypes towerType; // Type of tower
        private int damageDealt; // This is the amount of damage that the tower deals per attack
        private int targetingRange; // Firing range of tower
        private List<Enemy> enemiesInRange; // Queue of enemies to enemies that are within firing range
        private double fireRateSecs; // How often the tower should fire, in seconds
        private double nextFireTime = 0.0d; // The next fire time (in milliseconds)

        public Tower(Sprite sprite, towerTypes towerType)
            : base(sprite)
        {
            this.enemiesInRange = new List<Enemy>();
            this.towerType = towerType;
            setTowerProperties(towerType);
        }

        // Fire at the active target, if any
        public void fireAtActiveTarget()
        {
            #if DEBUG
            // Console.WriteLine("[Tower.fireAtActiveTarget()] Looking for active target");
            #endif
            if (this.enemiesInRange.Count > 0)
            {
                #if DEBUG
                Console.WriteLine("[Tower.fireAtActiveTarget()] Found active target with current strengh " + this.enemiesInRange[0].getStrength());
                #endif

                // Deal damage
                this.enemiesInRange[0].dealDamage(this.damageDealt);

                #if DEBUG
                Console.WriteLine("[Tower.fireAtActiveTarget()] Dealt " + this.damageDealt + " damage to the target, new strength is " + this.enemiesInRange[0].getStrength());
                #endif
            }
        }

        // Get the active target
        public Enemy getActiveTarget()
        {
            if (enemiesInRange.Count > 0)
            {
                return enemiesInRange[0];
            }
            else
            {
                return null;
            }
        }

        // Remove the active target
        public void removeActiveTarget()
        {
            enemiesInRange.RemoveAt(0);
        }

        // Iterate all enemies and add those within range to the tower's target list
        // Skip enemies that are either "dead" or already on the target list
        public void acquireNewTargets(List<Enemy> enemies)
        {
            foreach (Enemy e in enemies)
            {
                #if DEBUG
                // Console.WriteLine("[Tower.acquireNewTargets()] Found an enemy to consider");
                #endif

                // Is this enemy already "dead"?
                if (e.getStrength() <= 0)
                {
                    #if DEBUG
                    // Console.WriteLine("[Tower.acquireNewTargets()] This enemy is already \"dead\", skipping to next enemy (if any)");
                    #endif

                    // Skip to next enemy, if any
                    continue;
                }

                // Is this enemy already on the target list?
                if (enemiesInRange.Contains(e) == true)
                {
                    #if DEBUG
                    // Console.WriteLine("[Tower.acquireNewTargets()] This enemy is already on the target list, skipping to next enemy (if any)");
                    #endif

                    // Skip to next enemy, if any
                    continue;
                }

                // Is this enemy within range?
                int xDiff = e.getX() - this.x;
                int yDiff = e.getY() - this.y;
                int c = (xDiff * xDiff) + (yDiff * yDiff);
                #if DEBUG
                // Console.WriteLine("[Tower.acquireNewTargets()] " + c + " <= " + (this.targetingRange * this.targetingRange));
                #endif
                if (c <= (this.targetingRange * this.targetingRange))
                {
                    #if DEBUG
                    Console.WriteLine("[Tower.acquireNewTargets()] This enemy is within firing range, adding to target list");
                    #endif

                    // Add to target list
                    this.enemiesInRange.Add(e);
                }
            }
        }

        // Remove enemies that are on the target list but have fallen out of range
        public void dropTargetsOutOfRange()
        {
            List<Enemy> dropList = new List<Enemy>();
            foreach (Enemy e in enemiesInRange)
            {
                // Is this enemy no longer within range?
                int xDiff = e.getX() - this.x;
                int yDiff = e.getY() - this.y;
                int c = (xDiff * xDiff) + (yDiff * yDiff);
                #if DEBUG
                // Console.WriteLine("[Tower.dropTargetsOutOfRange()] " + c + " > " + (this.targetingRange * this.targetingRange));
                #endif
                if (c > (this.targetingRange * this.targetingRange))
                {
                    #if DEBUG
                    Console.WriteLine("[Tower.dropTargetsOutOfRange()] This enemy is no longer within firing range, dropping from target list");
                    #endif

                    // Add to drop from target list
                    dropList.Add(e);
                }
            }

            // Remove enemies to be dropped, if any
            foreach (Enemy e in dropList)
            {
                enemiesInRange.Remove(e);
            }
        }

        public double getNextFireTime()
        {
            return this.nextFireTime;
        }

        public void generateNextFireTime(GameTime gameTime)
        {
            this.nextFireTime = gameTime.TotalGameTime.TotalMilliseconds + (this.fireRateSecs * 1000);
        }

        public towerTypes getTowerType()
        {
            return this.towerType;
        }

        private void setTowerProperties(towerTypes towerType)
        {
            switch (towerType)
            {
                case towerTypes.HARM:
                    this.targetingRange = 150;
                    this.fireRateSecs = 2;
                    this.damageDealt = 5;
                    break;

                case towerTypes.SLOW:
                    this.targetingRange = 150;
                    this.fireRateSecs = 2;
                    this.damageDealt = 5;
                    break;

                case towerTypes.BLOCK:
                    this.targetingRange = 0;
                    this.fireRateSecs = 0;
                    this.damageDealt = 0;
                    break;
            }
        }
    }
}
