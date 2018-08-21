using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace SideScroller01
{
    enum GameState
    {
        MainMenu,
        HowToPlay,
        Playing
    }
    static class GameManager
    {

        public static GameState GameState = GameState.MainMenu;
        public static List<Level> Levels = new List<Level>();
        public static int CurrentLevel = 0;

        public static void Update(GameTime gT)
        {
            switch (GameState) {
                case GameState.MainMenu:
                    MenuManager.Update();
                    break;

                case GameState.HowToPlay:
                    // Await input to go back to main menu
                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.A)
                        || (InputHelper.WasKeyPressed(Keys.Space))
                        || (InputHelper.WasKeyPressed(Keys.Enter)))
                    {
                        GameState = GameState.MainMenu;
                    }
                    break;

                case GameState.Playing:
                   // Update the Level
                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Back)
                        || (InputHelper.WasKeyPressed(Keys.Escape))
                        || InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Start))
                    {
                        GameState = GameState.MainMenu;
                    }

                    Levels[CurrentLevel].Update(gT);
                    break;

            }
               
        }

        public static void Draw(SpriteBatch SB, SpriteBatch SBHUD)
        {
            switch (GameState)
            {
                case GameState.MainMenu:
                    MenuManager.Draw(SBHUD);
                    break;

                case GameState.HowToPlay:
                    SBHUD.Draw(Game1.SprHowToPlay, new Vector2(0, 0), Color.White);
                    //SBHUD.DrawString(Game1.FontSmall, "press any button to continue", new Vector2(180, 545), Color.CornflowerBlue);
                    break;

                case GameState.Playing:
                    Levels[CurrentLevel].Draw(SB, SBHUD);
                    break;


            }

        }

        public static void CreateLevels()
        {
            Levels.Clear(); // Removes all items from level, reset
            CurrentLevel = 0; // Reset GameManager's CurrentLevel to start a new game

            Level level;

            #region Level 01 - Outside of Jail

            level = new Level();

            // Level 1 Backgrounds
            level.AddBackgroundItem(Game1.SprStage1BGBack, Vector2.Zero, 0.1f, 0.95f);
            level.AddBackgroundItem(Game1.SprStage1BGBackB, new Vector2(Game1.SprStage1BGBack.Width, 0), 0.1f, 0.95f);
            level.AddBackgroundItem(Game1.SprStage1BGMain, Vector2.Zero, 1f, 0.90f);
            level.AddBackgroundItem(Game1.SprStage1BGMainB, new Vector2(Game1.SprStage1BGMain.Width, 0), 1f, 0.90f);
            level.AddBackgroundItem(Game1.SprStage1FGTreeBlur, new Vector2(-274, 0), 1f, 0.1f);
            level.AddBackgroundItem(Game1.SprStage1FGTreeBlur, new Vector2(1476, 0), 1f, 0.1f);

            // Define Level 1 starting PlayBounds
            level.PlayBounds = new Rectangle(0, 480, Game1.SCREEN_WIDTH, 120);

            // Level 1 Actors
            level.Actors.Add(new Player(new Vector2(-150, 550),
                                level, PlayerIndex.One));

            level.Player1 = level.Actors[level.Actors.Count - 1] as Player;
            level.Player1.FacingDir = DirectionTarget.Right;
            level.Player1.SetIntro01TargetPosition(new Vector2(270, 520));

            // Enemy
            AdonEnemy enemy_Adon;
            enemy_Adon = new AdonEnemy(new Vector2(650, 535), level);
            enemy_Adon.SetToWait(DirectionTarget.Left);
            level.Actors.Add(enemy_Adon);

            DeejayEnemy enemy;
            enemy = new DeejayEnemy(new Vector2(530, 500), level); // Laying Down Radio
            enemy.SetToLayingDown(DirectionTarget.Left);
            level.Actors.Add(enemy);

            enemy = new DeejayEnemy(new Vector2(710, 500), level); // Top 
            enemy.SetToWait(DirectionTarget.Left);
            level.Actors.Add(enemy);

            enemy = new DeejayEnemy(new Vector2(710, 570), level); // Bottom
            enemy.SetToWait(DirectionTarget.Left);
            level.Actors.Add(enemy);

            /////////////////////////////////////////////////////////////////

            TrashCan tc = new TrashCan(level, new Vector2(30, 500),
                new PickUpStone(level, Vector2.Zero));
            level.GameItems.Add(tc);
            tc = new TrashCan(level, new Vector2(90, 500),
                new PickUpHealthPack(level, Vector2.Zero, 40));
            level.GameItems.Add(tc);
            tc = new TrashCan(level, new Vector2(670, 500),
                new PickUpHealthPack(level, Vector2.Zero, 40));
            level.GameItems.Add(tc);
            tc = new TrashCan(level, new Vector2(710, 500),
                new PickUpHealthPack(level, Vector2.Zero, 40));
            level.GameItems.Add(tc);

            // Section 2
            tc = new TrashCan(level, new Vector2(1000, 485),
                new PickUpHealthPack(level, Vector2.Zero, 40));
            level.GameItems.Add(tc);
            tc = new TrashCan(level, new Vector2(1060, 485),
                new PickUpHealthPack(level, Vector2.Zero, 20));
            level.GameItems.Add(tc);
            tc = new TrashCan(level, new Vector2(1450, 490),
                new PickUpHealthPack(level, Vector2.Zero, 30));
            level.GameItems.Add(tc);
            
            // Section 3
            tc = new TrashCan(level, new Vector2(1940, 540),
                new PickUpHealthPack(level, Vector2.Zero, 60));
            level.GameItems.Add(tc);
            tc = new TrashCan(level, new Vector2(2460, 515),
                new PickUpHealthPack(level, Vector2.Zero, 40));
            level.GameItems.Add(tc);
            tc = new TrashCan(level, new Vector2(2500, 515),
                new PickUpHealthPack(level, Vector2.Zero, 10));
            level.GameItems.Add(tc);

            CreateLevel1CutScenes(level);
            
            Levels.Add(level);
            HUDManager.setLevel(level);

            Camera.Position = new Vector2(Game1.SCREEN_WIDTH / 2, Game1.SCREEN_HEIGHT / 2);

            #endregion

            #region Level 02 - Train Stattion

            level = new Level();

            // Level 2 Backgrounds
            level.AddBackgroundItem(Game1.SprStage2BGBack, Vector2.Zero, 0.1f, 0.95f);
            level.AddBackgroundItem(Game1.SprStage2BGMain, Vector2.Zero, 1f, 0.90f);

            // Define Level 2 starting PlayBounds
            level.PlayBounds = new Rectangle(0, 480, Game1.SCREEN_WIDTH, 120);

            // Create 2 enemies to the waiting for the players

            enemy = new DeejayEnemy(new Vector2(550, 480), level);
            enemy.SetToWait(DirectionTarget.Left);
            level.Actors.Add(enemy);

            enemy = new DeejayEnemy(new Vector2(660, 570), level);
            enemy.SetToWait(DirectionTarget.Left);
            level.Actors.Add(enemy);

            enemy = new DeejayEnemy(new Vector2(150, 490), level);
            enemy.SetToWait(DirectionTarget.Right);
            level.Actors.Add(enemy);

            enemy = new DeejayEnemy(new Vector2(170, 580), level);
            enemy.SetToWait(DirectionTarget.Right);
            level.Actors.Add(enemy);

            // Trash Can
            TrashCan tc_Level02 = new TrashCan(level, new Vector2(640, 480),
                new PickUpStone(level, Vector2.Zero));
            level.GameItems.Add(tc_Level02);
            TrashCan tc2_Level02 = new TrashCan(level, new Vector2(580, 475),
                new PickUpHealthPack(level, Vector2.Zero, 40));
            level.GameItems.Add(tc2_Level02);

            CreateLevel2CutScenes(level);

            Levels.Add(level);

            #endregion

        }

        static private void CreateLevel1CutScenes(Level level)
        {
            // CutScene Information
            CutScene scene;
            string line;
            //string voiceActingSound;
            Texture2D portrait;
            float timer;

            // SceneEvent Information
            SceneEvent sceneEvent;
            Trigger trigger;

            #region Level Introduction - Cody talks to Adon, seeks revenge
            // Creates TriggerHitBox close to the 2 enemies (Deejays)
            
            #region CutScene
            scene = new CutScene();
            
            // Line 1 
            line = "I'm coming for you!!";
            portrait = Game1.SprCutSceneCody01;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 2 
            line = "You will all die for what you have done!!!";
            portrait = Game1.SprCutSceneCody01;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 3 
            line = "Hey blondie, how delightful! Maybe this time you will stay quite";
            portrait = Game1.SprCutSceneAdon01;
            timer = 7f;
            scene.AddLine(line, portrait, timer);
            
            // Line 4 
            line = "Just pray that I don't find you!";
            portrait = Game1.SprCutSceneCody02;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 5 
            line = "Get him!!!!!";
            portrait = Game1.SprCutSceneAdon02;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);
            #endregion

            #region Post Intro SceneEvent

            sceneEvent = new SceneEvent();
            trigger = new TriggerHitBox(new Rectangle(500, 400, 300, 200));
            sceneEvent.AddTrigger(trigger);
            level.SceneEvents.Add(sceneEvent);
            
            #endregion
            
            #endregion

            #region First Cutscene - Cody talks to Adon, player can move,  
            // creates hitBox close to the 2 enemies on the right

            #region CutScene
            scene = new CutScene();

            // Line 1 - Who are you?
            line = "I'm gonna squash you!!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody02;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 2 - Who are you?
            line = "You all are going down!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody01;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 3 - Guys you all are going down, I'm coming for you Adon!
            line = "Hehe, this is going to be a BAD day for you!";
            //voiceCueName
            portrait = Game1.SprCutSceneDeejay02;
            timer = 7f;
            scene.AddLine(line, portrait, timer);

            // Line 4 - I'm ready!
            line = "COME ON!!!!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody02;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);

            #endregion

            #region Post Cutscene SceneEvent

            sceneEvent = new SceneEventActivateEnemies();
            trigger = new TriggerNoEnemies();
            sceneEvent.AddTrigger(trigger);
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Second After you defeat the 2 enemies, the enemies talk to you for a bit
            // HitBox Area trigger: Defeated the 2 enemies outside the jail breakout area, camera pans
            
            #region CutScene
            scene = new CutScene();

            // Line 1 - Who are you?
            line = "I'll take on all of you!!!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody02;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 2 - Who are you?
            line = "Fucking punks, so big but no brains...";
            //voiceCueName
            portrait = Game1.SprCutSceneDeejay03;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);

            #endregion

            #region Post Cutscene SceneEvent

            sceneEvent = new SceneEvent();
            trigger = new TriggerHitBoxNoCS(new Rectangle(960, 400, 200, 200));
            sceneEvent.AddTrigger(trigger);
            sceneEvent.AddPlayBounds(500);
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Third - After you tell the 2 enemies you beatup, a few things about fighting

            #region Post SceneEvent

            sceneEvent = new SceneEvent();
            trigger = new TriggerHitBoxNoCS(new Rectangle(1600, 400, 200, 200));
            sceneEvent.AddTrigger(trigger);
            sceneEvent.AddPlayBounds(500);
            sceneEvent.AddEnemeySpawner(
                new EnemySpawner(new Vector2(1141, 472), Game1.SprStage1FGDoorEntry01,
                    new RolentoEnemy(new Vector2(1135, 472), level)));

            sceneEvent.AddEnemy(new RolentoEnemy(DirectionTarget.Right, level));
           //sceneEvent.AddEnemy(new EnemyRanged(Direction.Left, level));
            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Left, level));
            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Right, level));
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Fourth - After you beat 3 Close Range, 1 Far Range

            #region Post SceneEvent

            sceneEvent = new SceneEvent();
            trigger = new TriggerNoEnemies();
            sceneEvent.AddTrigger(trigger);
            sceneEvent.AddEnemeySpawner(
                new EnemySpawner(new Vector2(1141, 472), Game1.SprStage1FGDoorEntry01,
                    new RolentoEnemy(new Vector2(1135, 472), level)));

            sceneEvent.AddEnemy(new RolentoEnemy(DirectionTarget.Left, level));
            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Left, level));
            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Right, level));
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Fifth - After the first enemy comes out of the door, with 2 more close and 1 far range
            // If player goes further to the hitbox, spawn 3 close and 1 far range!!!

            #region CutScene
            scene = new CutScene();

            // Line 1 - Where are you ADON!!!!
            line = "Where are you!!!!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody01;
            timer = 3f;
            scene.AddLine(line, portrait, timer);

            // Line 2 - ADOOONNN!!!!! Fucking bitch come out!!
            line = "Come out and fight!!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody02;
            timer = 4f;
            scene.AddLine(line, portrait, timer);

            // Line 3 - Over here Blondie
            line = "Over here BLONDIE!! Hahahahaa!!!";
            //voiceCueName
            portrait = Game1.SprCutSceneAdon01;
            timer = 3f;
            scene.AddLine(line, portrait, timer);

            // Line 4 - I'm ready!
            line = "HA!! Let's FINISH THIS COMEDY";
            //voiceCueName
            portrait = Game1.SprCutSceneAdon02;
            timer = 6f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);

            #endregion

            #region Post SceneEvent

            sceneEvent = new SceneEvent();
            trigger = new TriggerHitBoxNoCS(new Rectangle(2500, 400, 200, 200));
            sceneEvent.AddTrigger(trigger);
            sceneEvent.AddPlayBounds(1000);
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Six - After cutscene, spawn 4 more close and 1 range

            #region Post SceneEvent

            sceneEvent = new SceneEvent();
            trigger = new TriggerNoEnemies();
            sceneEvent.AddTrigger(trigger);
            sceneEvent.AddEnemy(new RolentoEnemy(DirectionTarget.Left, level));
            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Left, level));
            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Left, level));

            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Seven - After defeating all 5 enemies, Cutscene to next level

            #region CutScene
            scene = new CutScene();

            // Line 1 - Where are you ADON!!!!
            line = "Why are you running!!!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody02;
            timer = 3f;
            scene.AddLine(line, portrait, timer);

            // Line 2 - ADOOONNN!!!!! Fucking bitch come out!!
            line = "You can't hide from me!!!";
            //voiceCueName
            portrait = Game1.SprCutSceneCody01;
            timer = 4f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);

            #endregion

            #region Post SceneEvent

            sceneEvent = new SceneEvent();
            // Trigger to takes us to the Next Level
            trigger = new TriggerNextLevel();
            sceneEvent.AddTrigger(trigger);
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

        }
        
        static private void CreateLevel2CutScenes(Level level)
        {
            // CutScene Information
            CutScene scene;
            string line;
            //string voiceActingSound;
            Texture2D portrait;
            float timer;

            // SceneEvent Information
            SceneEvent sceneEvent;
            Trigger trigger;

            #region Level 2 Introduction
           
            #region CutScene
            scene = new CutScene();

            // Line 1 
            line = "Move or I'll crush you!";
            portrait = Game1.SprCutSceneCody01;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 2 
            line = "Time to take you out of your misery";
            portrait = Game1.SprCutSceneDeejay02;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 3 
            line = "There is still time to run, I mean... if your interested!";
            portrait = Game1.SprCutSceneCody01;
            timer = 7f;
            scene.AddLine(line, portrait, timer);

            // Line 4 
            line = "Enough talk, AHHHHH!!!";
            portrait = Game1.SprCutSceneDeejay01;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);
            #endregion

            #region Post Intro SceneEvent

            sceneEvent = new SceneEventActivateEnemies();
            // Trigger to takes us to the Next Level
            trigger = new TriggerNoEnemiesNoCS();
            sceneEvent.AddTrigger(trigger);
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Level 2 - 2nd Event: After you defeate the intro enemies (6) BG pans a bit more.
            // Hitbox created

            sceneEvent = new SceneEvent();
            trigger = new TriggerHitBoxNoCS(new Rectangle(700, 450, 100, 148));
            sceneEvent.AddPlayBounds(70);
            sceneEvent.AddTrigger(trigger);
            level.SceneEvents.Add(sceneEvent);
            
            #endregion

            #region Level 2 - 3rd Event: Spawn 4 enemies 1 range
            // Hitbox created

            sceneEvent = new SceneEvent();
            trigger = new TriggerNoEnemies();
            sceneEvent.AddTrigger(trigger);

            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Left, level));
            sceneEvent.AddEnemy(new DeejayEnemy(DirectionTarget.Right, level));
            sceneEvent.AddEnemy(new RolentoEnemy(DirectionTarget.Right, level));

            level.SceneEvents.Add(sceneEvent);

            #endregion

            #region Level 2 After the 4 close combat and 1 range enemies are defeated
            // Spawn ADON and CODY TALK SCENE - before fight

            #region CutScene
            scene = new CutScene();

            // Line 1 
            line = "Why all the running?!";
            portrait = Game1.SprCutSceneCody01;
            timer = 2f;
            scene.AddLine(line, portrait, timer);

            // Line 2 
            line = "I'm going to tear your throat and feed it to the pigs!";
            portrait = Game1.SprCutSceneAdon02;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 3 
            line = "Come and get me!!!!";
            portrait = Game1.SprCutSceneCody01;
            timer = 7f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);
            #endregion

            #region Post SceneEvent

            sceneEvent = new SceneEvent();
            // Trigger to takes us to the Next Level
            trigger = new TriggerNoEnemiesNoCS();
            sceneEvent.AddTrigger(trigger);
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion

            #region Level 2 - 4th Event: Spawn Final Boss for Level 2 - ADON
            // Hitbox created

            sceneEvent = new SceneEvent();
            trigger = new TriggerNoEnemies();
            sceneEvent.AddTrigger(trigger);

            sceneEvent.AddEnemy(new AdonEnemy(DirectionTarget.Neither, level));

            level.SceneEvents.Add(sceneEvent);

            #endregion

            #region Level 2 Final Event

            #region CutScene
            scene = new CutScene();

            // Line 1 
            line = "Where is she?!!";
            portrait = Game1.SprCutSceneCody01;
            timer = 3f;
            scene.AddLine(line, portrait, timer);

            // Line 2 
            line = "...";
            portrait = Game1.SprCutSceneCody02;
            timer = 3f;
            scene.AddLine(line, portrait, timer);

            // Line 3 
            line = "Tell me or I will finish you off right now!";
            portrait = Game1.SprCutSceneCody01;
            timer = 5f;
            scene.AddLine(line, portrait, timer);

            // Line 4 
            line = "We put her on a plane this morning...";
            portrait = Game1.SprCutSceneAdon01;
            timer = 4f;
            scene.AddLine(line, portrait, timer);

            // Line 5 
            line = "Bullshit!";
            portrait = Game1.SprCutSceneCody01;
            timer = 3f;
            scene.AddLine(line, portrait, timer);

            // Line 6 
            line = "The organizing principal for any society is war, its our way of life. You see fear is control, fear is money!";
            portrait = Game1.SprCutSceneAdon01;
            timer = 6f;
            scene.AddLine(line, portrait, timer);

            // Line 8 
            line = "I don't give shit! Where is she?!";
            portrait = Game1.SprCutSceneCody02;
            timer = 3f;
            scene.AddLine(line, portrait, timer);

            // Line 9 
            line = "The young men from your estate, so vicious, poor Elena wouldn't stand a...";
            portrait = Game1.SprCutSceneAdon01;
            timer = 6f;
            scene.AddLine(line, portrait, timer);

            level.CutScenes.Add(scene);
            #endregion

            #region Post SceneEvent

            sceneEvent = new SceneEvent();
            // Trigger to takes us to the Next Level
            trigger = new TriggerNextLevel();
            sceneEvent.AddTrigger(trigger);
            level.SceneEvents.Add(sceneEvent);

            #endregion

            #endregion


        }

        public static void GoToNextLevel()
        {
            CurrentLevel++;

            HUDManager.setLevel(Levels[CurrentLevel]);

            // Copy the needed details cross to the next level
            // Set the Player1 to the same Player1 as the last level
            Levels[CurrentLevel].Player1 = Levels[CurrentLevel - 1].Player1;
            // Make sure the Player1 is in the new Actor's list
            Levels[CurrentLevel].Actors.Add(Levels[CurrentLevel].Player1);
            // Set player1's inLevel
            Levels[CurrentLevel].Player1.InLevel = Levels[CurrentLevel];

            // Reset Player1 Draw details
            Levels[CurrentLevel].Player1.ResetIdleGraphic();

            Levels[CurrentLevel].Player1.Health = 300; // Maybe leave energy the same
            Levels[CurrentLevel].Player1.Position = new Vector2(-100, 440);
            Levels[CurrentLevel].Player1.SetIntro01TargetPosition(new Vector2(425, 520));
            Levels[CurrentLevel].Player1.State = PlayerState.Level01Intro;

            Levels[CurrentLevel].LevelState = LevelState.FadeIn;
            Camera.Position = new Vector2(Game1.SCREEN_WIDTH / 2, Game1.SCREEN_HEIGHT / 2);

            if (CurrentLevel == 2) // If Level 2 play Lvl 2 music
            {
                MusicManager.StopSong();
                MusicManager.PlaySong(Game1.MusicGame02);
            }

        }

    }
}
