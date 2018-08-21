using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller01
{
    class Trigger
    {
        public Trigger()
        { }

        public virtual void Update()
        {
        }

        public void ActivateTriggerNoCS()
        {
            GameManager.Levels[GameManager.CurrentLevel].ActivateSceneEvent();
        }

        public void ActivateTriggerWithCS()
        {
            Level level = GameManager.Levels[GameManager.CurrentLevel];

            level.LevelState = LevelState.CutScene;
            level.Player1.ResetIdleGraphic();

            level.CutScenes[level.CurrentCutScene].PlayFirstLine();
            MusicManager.ChangeToVolume(MusicManager.VolumeTarget);
        }
    }

    /// <summary>
    /// Triggers when player is the only actor left in the level Actor's list
    /// DOES Trigger a Cutscene
    /// </summary>
    class TriggerNoEnemies : Trigger
    {
        public TriggerNoEnemies()
            : base()
        {

        }

        public override void Update()
        {
            // Make sure Player is the ONLY Actor left in the Actors list
            if (GameManager.Levels[GameManager.CurrentLevel].Actors.Count == 1)
            {
                // There is only ONE player
                // If all other enemies are beaten up, then there will be only ONE actor
                // Is it the Player?
                if (GameManager.Levels[GameManager.CurrentLevel].Actors[0] as Player != null)
                {
                    // We know we are looking at the Player
                    this.ActivateTriggerWithCS();
                }
            }
        }


    }

    /// <summary>
    /// Triggers when player is the only actor left in the level Actor's list
    /// DOES NOT Trigger a Cutscene
    /// </summary>
    class TriggerNoEnemiesNoCS : Trigger
    {
        public TriggerNoEnemiesNoCS()
            : base()
        {

        }

        public override void Update()
        {
            // Make sure Player is the ONLY Actor left in the Actors list
            if (GameManager.Levels[GameManager.CurrentLevel].Actors.Count == 1)
            {
                // There is only ONE player
                // If all other enemies are beaten up, then there will be only ONE actor
                // Is it the Player?
                if (GameManager.Levels[GameManager.CurrentLevel].Actors[0] as Player != null)
                {
                    // We know we are looking at the Player
                    this.ActivateTriggerNoCS();
                }
            }
        }
    }

    /// <summary>
    /// Trigger when a player touching its hitbox
    /// DOES Trigger a Cutscene
    /// </summary>
    class TriggerHitBox : Trigger
    {
        Rectangle hitBox;

        public TriggerHitBox(Rectangle hitBox)
            : base()
        {
            this.hitBox = hitBox;
        }

        public override void Update()
        {
            Level level = GameManager.Levels[GameManager.CurrentLevel];
            
            // Convert PLayer's hitarea to a rectangle
            Rectangle playerHitBox = new Rectangle(
                (int)level.Player1.Position.X - level.Player1.HitArea,
                (int)level.Player1.Position.Y - 5,
                level.Player1.HitArea * 2, 10);

            if (playerHitBox.Intersects(this.hitBox))
            {
                this.ActivateTriggerWithCS();
            }
        }



    }


    /// <summary>
    /// Trigger when a player touching its hitbox
    /// DOES NOT trigger a Cutscene
    /// </summary>
    class TriggerHitBoxNoCS : Trigger
    {
        Rectangle hitBox;

        public TriggerHitBoxNoCS(Rectangle hitBox)
            : base()
        {
            this.hitBox = hitBox;
        }

        public override void Update()
        {
            Level level = GameManager.Levels[GameManager.CurrentLevel];

            // Convert PLayer's hitarea to a rectangle
            Rectangle playerHitBox = new Rectangle(
                (int)level.Player1.Position.X - level.Player1.HitArea,
                (int)level.Player1.Position.Y - 5,
                level.Player1.HitArea * 2, 10);

            if (playerHitBox.Intersects(this.hitBox))
            {
                this.ActivateTriggerNoCS();
            }
        }

 

    }

    /// <summary>
    /// Triggers when the player is the only actor left and player is touching a hotBox
    /// Will trigger Cutscene to play
    /// </summary>
    class TriggerNoEnemiesHitBox : Trigger
    {
        Rectangle hitBox;

        public TriggerNoEnemiesHitBox(Rectangle hitBox)
            : base()
        {
            this.hitBox = hitBox;
        }

        public override void Update()
        {
            Level level = GameManager.Levels[GameManager.CurrentLevel];
            
            // Convert PLayer's hitarea to a rectangle
            Rectangle playerHitBox = new Rectangle(
                (int)level.Player1.Position.X - level.Player1.HitArea,
                (int)level.Player1.Position.Y - 5,
                level.Player1.HitArea * 2, 10);

            if (playerHitBox.Intersects(this.hitBox))
            {
                // Make sure Player is the ONLY Actor left in the Actors list
                if (level.Actors.Count == 1)
                {
                    // There is only ONE player
                    // If all other enemies are beaten up, then there will be only ONE actor
                    // Is it the Player?
                    if (level.Actors[0] as Player != null)
                    {
                        // We know we are looking at the Player
                        this.ActivateTriggerWithCS();
                    }
                }
            }
        }

    }

    class TriggerNextLevel : Trigger
    {
        public TriggerNextLevel()
            : base()
        { }

        public override void Update()
        {
            // Check if there is a next level to go to
            if (GameManager.CurrentLevel + 1 < GameManager.Levels.Count)
            {
                // Yes you can fade out to next level
                GameManager.Levels[GameManager.CurrentLevel].LevelState = LevelState.FadeOut;
            }
            else
            {
                // Finished Final Level, Show Final Screen
                GameManager.Levels[GameManager.CurrentLevel].LevelState = LevelState.Completed;
            }


            
        }


    }

}
