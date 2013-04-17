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
        private int damageDealt; // This is the amount of damage that the tower deals per attack (if any)
        private float percentSlowDownDealt; // This is the percentage of slow down that this tower deals 
        private int targetingRange; // Firing range of tower
        private List<Enemy> enemiesInRange; // Queue of enemies to enemies that are within firing range
        private double fireRateSecs; // How often the tower should fire, in seconds
        private double nextFireTime = 0.0d; // The next fire time (in milliseconds)
        private bool isAreaEffect;
        private Color color;

        public Tower(Sprite sprite, towerTypes towerType)
            : base(sprite)
        {
            this.enemiesInRange = new List<Enemy>();
            this.towerType = towerType;
            setTowerProperties(towerType);
        }

        // Fire at the active targets, if any
        public void fireAtActiveTargets()
        {
            #if DEBUG
            // Console.WriteLine("[Tower.fireAtActiveTarget()] Looking for active targets");
            #endif

            List<Enemy> activeTargetList = this.getActiveTargets();
            List<Enemy> removalList = new List<Enemy>();
            if (activeTargetList != null)
            {
                foreach (Enemy e in activeTargetList)
                {
                    #if DEBUG
                    // Console.WriteLine("[Tower.fireAtActiveTarget()] Found active target with current strengh " + e.getStrength());
                    #endif

                    // Deal damage (if damage dealing tower)
                    if (this.damageDealt > 0)
                    {
                        // Render the animated bullet
                        GameController.towerShootEnemy(this, e);

                        // Deal the damage
                        e.dealDamage(this.damageDealt);

                        #if DEBUG
                        // Console.WriteLine("[Tower.fireAtActiveTarget()] Dealt " + this.damageDealt + " damage to the target, new strength is " + e.getStrength());
                        #endif
                    }

                    // Slow down (if slow down tower) if this target hasn't already been slowed down before 
                    if (this.percentSlowDownDealt > 0f && e.getHasBeenSlowedDown() == false)
                    {
                        e.slowDown(this.percentSlowDownDealt);
                        e.setHasBeenSlowedDown(true);

                        #if DEBUG
                        // Console.WriteLine("[Tower.fireAtActiveTarget()] Slowed down target");
                        #endif
                    }

                    // Zero strength?  If so then add to the removal list
                    if (e.getStrength() <= 0)
                    {
                        removalList.Add(e);
                    }
                }
            }

            // Remove enemies that were added to the removal list
            foreach (Enemy e in removalList)
            {
                enemiesInRange.Remove(e);
            }
        }

        // Get the active targets
        public List<Enemy> getActiveTargets()
        {
            // Are there any enemies in range?
            if (enemiesInRange.Count > 0)
            {
                // Is this an area effect tower?
                // If so then return the first enemy on the active targets list
                // Otherwise return the entire active targets list
                if (this.isAreaEffect == false)
                {
                    List<Enemy> _enemyInRange = new List<Enemy>();
                    _enemyInRange.Add(enemiesInRange[0]);
                    return _enemyInRange;
                }
                else
                {
                    return enemiesInRange;
                }
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
                    // Console.WriteLine("[Tower.acquireNewTargets()] This enemy is within firing range, adding to target list");
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
                    // Console.WriteLine("[Tower.dropTargetsOutOfRange()] This enemy is no longer within firing range, dropping from target list");
                    #endif

                    // Do we need to speed this enemy back up?
                    if (this.percentSlowDownDealt > 0f && e.getHasBeenSlowedDown() == true)
                    {
                        e.speedUp(this.percentSlowDownDealt);
                        e.setHasBeenSlowedDown(false);

                        #if DEBUG
                        // Console.WriteLine("[Tower.fireAtActiveTarget()] Sped up target");
                        #endif
                    }


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

        public int getDamageDealt()
        {
            return this.damageDealt;
        }

        public float getPercentSlowDownDealt()
        {
            return this.percentSlowDownDealt;
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
                    this.fireRateSecs = 1;
                    this.damageDealt = 5;
                    this.percentSlowDownDealt = 0f;
                    this.isAreaEffect = false;
                    this.setSprite(new Sprite(ContentStore.getTexture("spr_yell")));
                    this.color = Color.Orange;
                    break;

                case towerTypes.SLOW:
                    this.targetingRange = 75;
                    this.fireRateSecs = .5;
                    this.damageDealt = 1;
                    this.isAreaEffect = true;
                    this.percentSlowDownDealt = 30.0f;
                    this.setSprite(new Sprite(ContentStore.getTexture("spr_slow")));
                    this.color = Color.Blue;
                    break;

                case towerTypes.BLOCK:
                    this.targetingRange = 0;
                    this.fireRateSecs = 0;
                    this.damageDealt = 0;
                    this.isAreaEffect = false;
                    this.percentSlowDownDealt = 0f;
                    this.setSprite(new Sprite(ContentStore.getTexture("spr_blockTower")));
                    this.color = Color.Silver;
                    break;
            }
        }

        public static int getTowerCost(towerTypes type)
        {
            switch (type)
            {
                case towerTypes.HARM:
                    return 50;
                case towerTypes.SLOW:
                    return 100;
                case towerTypes.BLOCK:
                    return 10;
            }

            return 0;
        }

        public void remove()
        {
            GameController.removeTower(this);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            base.draw(sb,color);
        }
    }
}
