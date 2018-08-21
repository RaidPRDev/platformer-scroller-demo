using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{
    static class HUDManager
    {
        public static float Opacity;
        public static float HealthOpacity = 0;
        static Level currentLevel;
        static int HealthBarWidth = 200;
        static int HealthBarHeight = 16;

        public static void setLevel(Level level)
        {
            currentLevel = level;
        }

        public static void DrawHUD(SpriteBatch SBHUD)
        {

            if (currentLevel.Player1.Health <= 40)
            {
                if (HealthOpacity <= 0.2f)
                    HealthOpacity += 0.003f;
                
                SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT),
                        new Color(Color.Red, HealthOpacity));
            }
            else if (currentLevel.Player1.Health >= 40)
            {
                if (HealthOpacity > 0f)
                    HealthOpacity -= 0.003f;

                SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT),
                        new Color(Color.Red, HealthOpacity));

            }
            
            // Health Bar
            // Draw the yellow bar
            float percent = currentLevel.Player1.Health / Player.STARTING_HEALTH;
            int drawWidth = (int)(percent * HealthBarWidth);

            SBHUD.Draw(Game1.SprSinglePixel, new Vector2(66, Game1.SCREEN_HEIGHT - 45),
                new Rectangle(0,0,drawWidth, HealthBarHeight), Color.Yellow);

            // Draw Player One Avatar
            SBHUD.Draw(Game1.SprPlayerBarHUD, new Vector2(10, Game1.SCREEN_HEIGHT - Game1.SprPlayerBarHUD.Height), Color.White);

            // Player 1 Text
            SBHUD.DrawString(Game1.FontSmall, "PLAYER 1", new Vector2(32, Game1.SCREEN_HEIGHT - 24), Color.Black,
                0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            SBHUD.DrawString(Game1.FontSmall, "PLAYER 1", new Vector2(30, Game1.SCREEN_HEIGHT - 22), Color.White,
                0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // DEBUG VARIABLES //////////////////////////////////////////////////////////////////////////////////
            // Draw Black Bar
            /*
            SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0, 0, Game1.SCREEN_WIDTH, 140), new Color(Color.Black, 0.4f));

            float text_scale = 0.6f;

            SBHUD.DrawString(Game1.FontSmall, "CamX: " + Game1.cam._pos.X,
                new Vector2(20, 20), Color.CornflowerBlue, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);

            SBHUD.DrawString(Game1.FontSmall, "CamY: " + Game1.cam._pos.Y,
                new Vector2(200, 20), Color.CornflowerBlue, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);

            SBHUD.DrawString(Game1.FontSmall, "CamZoom: " + Game1.cam._zoom,
                new Vector2(360, 20), Color.CornflowerBlue, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);

            SBHUD.DrawString(Game1.FontSmall, "PlayerX: " + currentLevel.Player1.Position.X,
                new Vector2(20, 40), Color.CornflowerBlue, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);

            SBHUD.DrawString(Game1.FontSmall, "PlayerY: " + currentLevel.Player1.Position.Y,
                new Vector2(200, 40), Color.CornflowerBlue, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);

            SBHUD.DrawString(Game1.FontSmall, "BloomIntensity: " + Game1.bloom.Settings.BloomIntensity,
                new Vector2(20, 60), Color.CornflowerBlue, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);
            SBHUD.DrawString(Game1.FontSmall, "bloomIntensityIndex: " + Game1.bloomIntensityIndex,
                new Vector2(300, 60), Color.CornflowerBlue, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            */

            // If player carrying rocks draw in hud
            if (currentLevel.Player1.CarryingRock)
            {
                SBHUD.Draw(Game1.SprRocks, new Vector2(300, Game1.SCREEN_HEIGHT - 46), Color.White);
            }

        }

        public static void DrawContinueScreen(SpriteBatch SBHUD, float timeRemaining)
        {
            if (Opacity <= 0.4f) 
                Opacity += 0.003f;

            SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0,0,Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT), 
                        new Color(Color.Red, Opacity));
            
            // Current Number
            int time = (int)timeRemaining;
            SBHUD.DrawString(Game1.FontLarge, "CONTINUE?", new Vector2(120, 80), Color.Red);
            SBHUD.DrawString(Game1.FontLarge, time.ToString(), new Vector2(360, 190), Color.Gold);


        }

        public static void DrawGameOverScreen(SpriteBatch SBHUD)
        {
            if (Opacity <= 0.85f)
                Opacity += 0.003f;

            SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT),
            new Color(Color.Red, Opacity));

            SBHUD.DrawString(Game1.FontLarge,"GAME OVER", new Vector2(120, 80), Color.White);
            SBHUD.DrawString(Game1.FontLarge, "You Died", new Vector2(275, 250),
                Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            SBHUD.DrawString(Game1.FontLarge, "Press START to return to", new Vector2(160, 400),
                Color.White, 0f, Vector2.Zero, 0.44f, SpriteEffects.None, 0f);
            SBHUD.DrawString(Game1.FontSmall, "the MAIN MENU", new Vector2(280, 450), Color.White );
        }

        public static void DrawScreenFade(SpriteBatch SBHUD)
        {
            // Draw the levels fade, if level is fading
            SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0,0,Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT),
                new Color(Color.Black, Opacity));
        }

        public static void DrawGameCompleted(SpriteBatch SBHUD)
        {
            if (Opacity <= 1f)
                Opacity += 0.003f;

            SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT),
                new Color(Color.Black, Opacity));

            SBHUD.DrawString(Game1.FontLarge, "END OF DEMO", new Vector2((Game1.SCREEN_WIDTH / 2)-100, (Game1.SCREEN_HEIGHT/2)-150), 
                new Color(Color.White, Opacity), 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);

            SBHUD.DrawString(Game1.FontLarge, "Thank you for playing!", new Vector2((Game1.SCREEN_WIDTH / 2) - 100, (Game1.SCREEN_HEIGHT / 2) - 50), 
                new Color(Color.White, Opacity), 0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);

            //SBHUD.DrawString(Game1.FontLarge, "To be ", new Vector2(145, 480), 
              //  new Color(Color.White, Opacity), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

        }
    }
}
