using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{
    enum MenuState
    {
        TitleScreen,
        MainMenu

    }
    struct MenuItem
    {
        string text;
        Vector2 position;

        public MenuItem(string text, Vector2 position)
        {
            this.text = text;
            this.position = position;
        }

        public void Draw(SpriteBatch SBHUD, Color drawColor)
        {
            SBHUD.DrawString(Game1.FontSmall, text, position, drawColor);
        }


    }
    struct TextItem
    {
        string text;
        Vector2 position;

        public TextItem(string text, Vector2 position)
        {
            this.text = text;
            this.position = position;
        }

        public void Draw(SpriteBatch SBHUD, Color drawColor)
        {
            SBHUD.DrawString(Game1.FontSmall, text, position, drawColor);
        }


    }

    static class MenuManager
    {
        static Color colorStandard = Color.White;
        static Color colorSelected = Color.LightBlue;

        static List<MenuItem> menuItems = new List<MenuItem>();
        static List<TextItem> textItems = new List<TextItem>();
        static int currentMenuItem = 0;
        public static MenuState MenuState = MenuState.TitleScreen;

        public static void Update()
        {
            switch (MenuState)
            {
                case MenuState.TitleScreen:

                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.A)
                        || InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.Start)
                        || InputHelper.WasKeyPressed(Keys.Space)
                        || InputHelper.WasKeyPressed(Keys.Enter))
                    {
                        MenuState = MenuState.MainMenu;                   
                    }
                    break;

                case MenuState.MainMenu:
                    // Menu Navigation
                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.DPadUp)
                        || (InputHelper.NGS[(int)PlayerIndex.One].ThumbSticks.Left.Y < 0.3 && 
                            InputHelper.NGS[(int)PlayerIndex.One].ThumbSticks.Left.Y > 0.3)
                        || InputHelper.WasKeyPressed(Keys.Up))
                    {
                        currentMenuItem--;
                        if (currentMenuItem < 0)
                            currentMenuItem = menuItems.Count - 1;
                        SoundManager.PlaySound("PickUpItem");
                    }

                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.DPadDown)
                        || (InputHelper.NGS[(int)PlayerIndex.One].ThumbSticks.Left.Y < -0.3 &&
                            InputHelper.NGS[(int)PlayerIndex.One].ThumbSticks.Left.Y > -0.3)
                        || InputHelper.WasKeyPressed(Keys.Down))
                    {
                        currentMenuItem++;
                        if (currentMenuItem >= menuItems.Count)
                            currentMenuItem = 0;
                        SoundManager.PlaySound("PickUpItem");
                    }
                            

                    //Menu Item Actions
                    if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.A)
                        || (InputHelper.WasKeyPressed(Keys.Space))
                        || (InputHelper.WasKeyPressed(Keys.Enter)))
                    {
                        switch (currentMenuItem) 
                        {
                            case 0: // Begin Game
                                
                                GameManager.GameState = GameState.Playing;
                                GameManager.CreateLevels();
                                MusicManager.StopSong();
                                MusicManager.PlaySong(Game1.MusicGame01);
                                MusicManager.SetRepeating(true);
                                
                                break;
                            case 1: // How To Play
                                GameManager.GameState = GameState.HowToPlay;
                                break;
                            case 2: // Exit Game
                                Game1.ExitGame();
                                break;

                        }

                    }
                    break;

            }

        }

        public static void Draw(SpriteBatch SBHUD)
        {
            // Draw Background Image
            SBHUD.Draw(Game1.SprTitleScreenBackground, new Rectangle(0,0,Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT),
                new Color(Color.White, 1f));

            switch (MenuState)
            {
                case MenuState.TitleScreen:
                    SBHUD.Draw(Game1.SprTitleScreenCharOff, new Vector2(500, 60), Color.White);
                    SBHUD.DrawString(Game1.FontSmall, "Press Start", new Vector2(560, 550), Color.White);
                    
                    break;

                case MenuState.MainMenu:
                    SBHUD.Draw(Game1.SprSinglePixel, new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT),
                        new Color(Color.Black, 0.5f));
                    SBHUD.Draw(Game1.SprTitleScreenCharOn, new Vector2(502, 60), Color.White);
                    
                    for (int i = 0; i < menuItems.Count; i++)
                    {
                        if (i == currentMenuItem)
                            menuItems[i].Draw(SBHUD, colorSelected);
                        else
                            menuItems[i].Draw(SBHUD, colorStandard);
                    }

                    break;


            } 
        }

        public static void CreateMenuItems()
        {
            menuItems.Add(new MenuItem("Begin Game", new Vector2(45, 50)));
            menuItems.Add(new MenuItem("How To Play", new Vector2(45, 120)));
            menuItems.Add(new MenuItem("Exit Game", new Vector2(45, 190)));

            // Play Music
            MusicManager.StopSong();
            MusicManager.PlaySong(Game1.MusicTitleScreen);
            MusicManager.SetRepeating(true);


        }

        public static void DrawText(string text, float x, float y)
        {
            textItems.Add(new TextItem(text, new Vector2(x, y)));

        }

          
    }
}
