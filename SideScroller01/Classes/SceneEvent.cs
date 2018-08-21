using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller01
{
    class SceneEvent
    {
        Trigger triggerToAdd;
        int AddToPlayBounds;
        List<EnemySpawner> enemySpawnersToAdd;
        List<Actor> enemiesToAdd;

        public SceneEvent()
        {
            triggerToAdd = null;
            AddToPlayBounds = 0;
            enemySpawnersToAdd = new List<EnemySpawner>();
            enemiesToAdd = new List<Actor>();
        }

        public void AddTrigger(Trigger trigger)
        {
            triggerToAdd = trigger;
        }

        public void AddPlayBounds(int howMuch)
        {
            this.AddToPlayBounds = howMuch;
        }

        public void AddEnemeySpawner(EnemySpawner spawner)
        {
            this.enemySpawnersToAdd.Add(spawner);
        }

        public void AddEnemy(Actor enemy)
        {
            this.enemiesToAdd.Add(enemy);
        }

        public virtual void Activate(Level level)
        {
            // Add in the trigger to the level
            level.CurrentTrigger = this.triggerToAdd;

            // Add to the play bounds
            level.AddToPlayBounds(this.AddToPlayBounds);

            // Add the enemy spawners to level
            for (int i = 0; i < enemySpawnersToAdd.Count; i++)
            {
                level.EnemySpawners.Add(enemySpawnersToAdd[i]);
            }
            foreach (Actor enemy in enemiesToAdd)
            {
                Level.GetStartSidePosition(enemy, level);
                level.Actors.Add(enemy);
            }


        }
    }

    class SceneEventActivateEnemies : SceneEvent
    {
        public SceneEventActivateEnemies()
            : base() { }

        public override void Activate(Level level)
        {
            // Make all the waiting enemies start fighting
            for (int i = 0; i < level.Actors.Count; i++)
            {
                //EnemyClose enemy = level.Actors[i] as EnemyClose;
                DeejayEnemy enemy_Deejay = level.Actors[i] as DeejayEnemy;

                if (enemy_Deejay != null)
                {
                    if (enemy_Deejay.GetState()) // If set to TRUE, enemy is laying down
                        enemy_Deejay.GetUpAnimation();
                    else
                        enemy_Deejay.ResetIdleGraphic();
                }

                AdonEnemy enemy_Adon = level.Actors[i] as AdonEnemy;

                if (enemy_Adon != null)
                {
                    enemy_Adon.SetIntro01TargetPosition(new Vector2(950, 550));
                    //enemy_Adon.ResetIdleGraphic();
                }



            }
         
            base.Activate(level);
        }
    }
}
