using System;
using System.Diagnostics;
using BloomPostprocess;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SideScroller01.Classes.Scene;

namespace SideScroller01
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch spriteBatchHUD;

        private bool isLoading = false;

        public const int SCREEN_WIDTH = 800;
        public const int SCREEN_HEIGHT = 600;
        public static Random Random;

        static bool exitGame;

        public static SpriteFont FontSmall;
        public static SpriteFont FontLarge;

        public static Camera cam = new Camera();
        public static float _zoom;

        // BLOOM
        public static Bloom bloom;
        public static int bloomSettingsIndex = 0;
        public static float bloomIntensityIndex = 1f;

        public Game1()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = SCREEN_WIDTH,
                PreferredBackBufferHeight = SCREEN_HEIGHT,
                IsFullScreen = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine("Game.Initialize()");

            isLoading = true;
            exitGame = false;
            IsMouseVisible = true;
            Window.Title = "Side-Scroller";

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatchHUD = new SpriteBatch(GraphicsDevice);
            bloom = new Bloom(GraphicsDevice, spriteBatch);
         
            base.Initialize();
        }

        public SpriteBatch GetSpriteBatch
        {
            get { return spriteBatch; }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            isLoading = true;

            bloom.LoadContent(Content);

            Random = new Random();

            FontLarge = Content.Load<SpriteFont>(@"Fonts\FontLarge");
            FontSmall = Content.Load<SpriteFont>(@"Fonts\FontSmall");

            TextureManager.Initialize(Content);
            MusicManager.Initialize(Content);
            SoundManager.Initialize(Content);
            GameManager.Initialize();
            MenuManager.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape) || exitGame)
                Exit();

            if (isLoading)
            {
                base.Update(gameTime);
            };

            // Switch to the next bloom settings preset?
            if (InputHelper.WasKeyPressed(Keys.B))
            {
                bloomSettingsIndex = (bloomSettingsIndex + 1) % BloomSettings.PresetSettings.Length;
                bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
            }
         
            if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0.00001f)
                _zoom = 1.5f;

            if (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0.00001f)
                _zoom = 1f;

            cam.UpdateCameraZoom(_zoom, gameTime);
            // bloom.UpdateBloom(bloomIntensityIndex, gameTime);

            InputHelper.UpdateStates();
            GameManager.Update(gameTime);
            MusicManager.Update();

            // Just for fun - here I've added a pulse animation by adjusting the bloom saturation:
            /*BloomSatPulse += bloomSatDir;
            if (BloomSatPulse > 2.5f) bloomSatDir = -0.09f;
            if (BloomSatPulse < 0.1f) bloomSatDir = 0.09f;
            bloom.Settings.BloomSaturation = BloomSatPulse;*/

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Debug.WriteLine("Game.Draw()");

            bloom.BeginDraw();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.get_transformation(GraphicsDevice));

            SamplerState sState = new SamplerState
            {
                Filter = TextureFilter.Point
            };
            GraphicsDevice.SamplerStates[0] = sState;

            spriteBatchHUD.Begin();
            GameManager.Draw(spriteBatch, spriteBatchHUD);
            spriteBatch.End();
            spriteBatchHUD.End();

            bloom.EndDraw();

            base.Draw(gameTime);
        }

        public static void ExitGame()
        {
            exitGame = true;
        }

        public void SetWindowSize(int x, int y)
        {
            graphics.PreferredBackBufferWidth = x;
            graphics.PreferredBackBufferHeight = y;
            graphics.ApplyChanges();
        }
    }
}