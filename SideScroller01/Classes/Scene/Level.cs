using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Objects;
using SideScroller01.Classes.Objects.Enemies;
using SideScroller01.Classes.Objects.Items.Base;

namespace SideScroller01.Classes.Scene
{
    enum LevelState
    {
        Intro,
        CutScene,        
        Playing,
        Continue,
        GameOver,
        Completed,

        FadeOut,
        FadeIn
    }
    
    class Level
    {
        struct BackgroundItem
        {
            Texture2D texture;
            Vector2 position;
            float speed;
            float layerDepth;

            public BackgroundItem(Texture2D texture, Vector2 position,
                float speed, float layerDepth)
            {
                this.texture = texture;
                this.position = position;
                this.speed = speed;
                this.position = position;
                this.layerDepth = layerDepth;

            }

            public void Draw(SpriteBatch SB)
            {
                SB.Draw(this.texture, Camera.GetScreenPosition(this.position) * this.speed, null, Color.White, 0f,
                    Vector2.Zero, 1f, SpriteEffects.None, this.layerDepth);

            }
        }

        List<BackgroundItem> backgrounds;
        public List<Actor> Actors;
        public List<ActorSpawner> EnemySpawners;
        public List<GameItem> GameItems;
        public Rectangle PlayBounds;
        public Player Player1;
        public LevelState LevelState;
        public float TimerContinue;

        // CutScenes
        public List<CutScene> CutScenes;
        public int CurrentCutScene;
        public Trigger CurrentTrigger;
        public List<SceneEvent> SceneEvents;
        public int CurrentSceneEvent;
        
        
        public Level()
        {
            this.LevelState = LevelState.Playing;

            backgrounds = new List<BackgroundItem>();
            Actors = new List<Actor>();
            EnemySpawners = new List<ActorSpawner>();
            GameItems = new List<GameItem>();
            CutScenes = new List<CutScene>();
            CurrentCutScene = 0;
            SceneEvents = new List<SceneEvent>();
            CurrentSceneEvent = 0;

        }

        public void Update(GameTime gT)
        {
            switch (this.LevelState)
            {
                #region Intro

                case LevelState.Intro:
                    Player1.Update(gT);
                                  
                    break;
                #endregion

                #region Cutscene

                case LevelState.CutScene:
                    CutScenes[CurrentCutScene].PlayScene(gT);
                    break;

                #endregion
                    
                #region Playing
                case LevelState.Playing:
                    // Update the Actors
                    for (int i = 0; i < Actors.Count; i++)
                        Actors[i].Update(gT);

                    // Update Game Items
                    for (int i = 0; i < GameItems.Count; i++)
                        GameItems[i].Update(gT);

                    // Update Enemy Spawners
                    for (int i = 0; i < EnemySpawners.Count; i++)
                        EnemySpawners[i].Update(gT);

                    // Update Camera Position
                    UpdateCameraPosition(gT);

                    // Update Trigger
                    this.CurrentTrigger.Update();

                    // Extend the play area?
                    if (InputHelper.WasKeyPressed(Keys.Q))
                        AddToPlayBounds(150);

                    break;
                #endregion

                #region Continue

                case LevelState.Continue:
                    TimerContinue -= (float)gT.ElapsedGameTime.TotalSeconds;
                    // TimerContinue Finished
                    if (TimerContinue <= 0)
                    {
                        // Out of time, Game Over
                        this.LevelState = LevelState.GameOver;
                    }
                    
                    //
                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.A) ||
                       InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.B) ||
                       InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.X) ||
                       InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Y) ||
                       InputHelper.WasKeyPressed(Keys.NumPad7) ||
                       InputHelper.WasKeyPressed(Keys.NumPad8))
                    {
                        this.TimerContinue--;
                        SoundManager.PlaySound("PickUpItem");
                        

                    }

                    // Has player pressed Start to continue
                    //if (InputHelper.WasKeyPressed(Keys.Space)) 
                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Start)
                        || InputHelper.WasKeyPressed(Keys.Enter)
                        || InputHelper.WasKeyPressed(Keys.Space))
                    {
                        SoundManager.PlaySound("CrashGlass");
                        SoundManager.PlaySound("CodyWins");
                        this.Player1.Continue();
                        this.LevelState = LevelState.Playing;

                    }
                    break;

                #endregion

                #region GameOver

                case LevelState.GameOver:
                    // Has player pressed Start to continue
                    //if (InputHelper.WasKeyPressed(Keys.Space)) 
                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Start) ||
                        InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.A) ||
                        InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.B) ||
                        InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.X) ||
                        InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Y) ||
                        InputHelper.WasKeyPressed(Keys.Space) ||
                        InputHelper.WasKeyPressed(Keys.Enter))
                    {
                        SoundManager.PlaySound("CrashGlass");
                        SoundManager.PlaySound("CodyWins");
                        GameManager.GameState = GameState.MainMenu;

                    }
                    
                    break;

                #endregion

                #region Completed Game
                case LevelState.Completed:

                    if (InputHelper.WasKeyPressed(Keys.Space)
                        || InputHelper.WasKeyPressed(Keys.Enter)
                        || InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.A)
                        || InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Start))
                    {
                        GameManager.GameState = GameState.MainMenu;
                        MusicManager.PlaySong("music02");
                        SoundManager.PlaySound("CrashGlass");
                        SoundManager.PlaySound("GetOutOfMyFace");
                        MusicManager.ChangeToVolume(MusicManager.CurrentVolume);
                    }


                    break;
                    
                #endregion

                #region FadeIn
                case LevelState.FadeIn:
                    HUDManager.Opacity -= 0.008f;
                    if (HUDManager.Opacity <= 0) {
                        HUDManager.Opacity = 0;
                        LevelState = LevelState.Intro;

                    }
                    break;


                #endregion

                #region FadeOut
                case LevelState.FadeOut:
                    
                    HUDManager.Opacity += 0.008f;
                    if (HUDManager.Opacity >= 1f) {
                        HUDManager.Opacity = 1;
                        GameManager.GoToNextLevel();

                    }
                    break;



                #endregion


            }
        }

        public void Draw(SpriteBatch SB, SpriteBatch SBHUD)
        {
            #region Always draw me

            // Draw Backgrounds
            for (int i = 0; i < backgrounds.Count; i++)
                backgrounds[i].Draw(SB);

            // Draw Actors
            for (int i = 0; i < Actors.Count; i++)
                Actors[i].Draw(SB);

            // Draw Enemy Spawners
            for (int i = 0; i < EnemySpawners.Count; i++)
                EnemySpawners[i].Draw(SB);

            // Draw Game Items
            for (int i = 0; i < GameItems.Count; i++)
                GameItems[i].Draw(SB);

            HUDManager.DrawHUD(SBHUD);

            #endregion

            if (this.LevelState == LevelState.CutScene)
                CutScenes[CurrentCutScene].Draw(SBHUD);

            if (this.LevelState == LevelState.Continue)
                HUDManager.DrawContinueScreen(SBHUD, TimerContinue);

            if (this.LevelState == LevelState.GameOver)
                HUDManager.DrawGameOverScreen(SBHUD);

            if (this.LevelState == LevelState.FadeIn
                || this.LevelState == LevelState.FadeOut)
            {
                HUDManager.DrawScreenFade(SBHUD);

            }

            if (this.LevelState == LevelState.Completed)
                HUDManager.DrawGameCompleted(SBHUD);


        }

        public void UpdateCameraPosition(GameTime gT)
        {
            float next;
            float endX;

            endX = Player1.Position.X + Camera.MovementSpeed;

            if (Player1.Position.X > Camera.Position.X)
            {

                if (Player1.Position.X != endX)
                {

                    next = (Player1.Position.X - Camera.Position.X) * Camera.MovementSpeed * Camera.Elasticity;
                    Camera.Position.X += next;

                    if (Camera.Position.X + Game1.SCREEN_WIDTH / 2 > this.PlayBounds.Right)
                        Camera.Position.X = PlayBounds.Right - Game1.SCREEN_WIDTH / 2;


                }

            }
        }

      /*  private void UpdateCameraPosition()
        {
            if (Player1.Position.X > Camera.Position.X)
            {

                Camera.Position.X += 3 ;


                if (Camera.Position.X + Game1.SCREEN_WIDTH / 2 > this.PlayBounds.Right)
                    Camera.Position.X = PlayBounds.Right - Game1.SCREEN_WIDTH / 2;

            }
        }
        */


        public void AddBackgroundItem(Texture2D texture, Vector2 position,
            float speed, float layerDepth)
        {
            backgrounds.Add(new BackgroundItem(texture, position, speed, layerDepth));
        }

        public void AddToPlayBounds(int howMuch)
        {
            this.PlayBounds.Width += howMuch;

        }

        public void ActivateSceneEvent()
        {
            SceneEvents[CurrentSceneEvent].Activate(this);

            // Next Scene Event to happen next time
            CurrentSceneEvent++;

            this.LevelState = LevelState.Playing;
            MusicManager.ChangeToVolume(MusicManager.CurrentVolume);
        }

        public static void GetStartSidePosition(Actor enemy, Level level)
        {
            if (enemy.FacingDir == DirectionTarget.Left)  // 80 is the amount half of the player
                enemy.Position = new Vector2(Camera.Position.X - Game1.SCREEN_WIDTH / 2 - 80,
                    Game1.Random.Next(level.PlayBounds.Height) + level.PlayBounds.Top);

            else if (enemy.FacingDir == DirectionTarget.Right)  // 80 is the amount half of the player
                enemy.Position = new Vector2(Camera.Position.X + Game1.SCREEN_WIDTH / 2 + 80,
                    Game1.Random.Next(level.PlayBounds.Height) + level.PlayBounds.Top);

            else // Not left nor right, leaves only "Neither"... picking at Random
            {
                if (Game1.Random.NextDouble() >= 0.5d)
                {
                    // Picked Left
                    enemy.Position = new Vector2(Camera.Position.X - Game1.SCREEN_WIDTH / 2 - 80,
                        Game1.Random.Next(level.PlayBounds.Height) + level.PlayBounds.Top);
                }
                else
                {
                    // Picked Right
                    enemy.Position = new Vector2(Camera.Position.X + Game1.SCREEN_WIDTH / 2 + 80,
                        Game1.Random.Next(level.PlayBounds.Height) + level.PlayBounds.Top);
                }
            }
        }
    }
}
