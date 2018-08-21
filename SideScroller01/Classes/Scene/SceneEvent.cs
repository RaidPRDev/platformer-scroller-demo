using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller01.Classes.Objects;
using SideScroller01.Classes.Objects.Enemies;
using SideScroller01.Classes.Objects.Items.Base;

namespace SideScroller01.Classes.Scene
{
    class SceneEvent
    {
        Trigger triggerToAdd;
        int AddToPlayBounds;
        List<ActorSpawner> spawnersToAdd;
        List<Actor> enemiesToAdd;

        public SceneEvent()
        {
            triggerToAdd = null;
            AddToPlayBounds = 0;
            spawnersToAdd = new List<ActorSpawner>();
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

        public void AddSpawner(ActorSpawner spawner)
        {
            this.spawnersToAdd.Add(spawner);
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

            for (int i = 0; i < spawnersToAdd.Count; i++)
            {
                level.EnemySpawners.Add(spawnersToAdd[i]);
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
