using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SideScroller01.Classes.Scene
{
    class CutScene
    {
        float currentTime;

        List<string> lines;
        //List<string> voiceActing;
        List<Texture2D> portraits;
        List<float> timers;

        int currentLine;

        Texture2D singlePixel;
        Texture2D avatarImage;

        public CutScene()
        {
            lines = new List<string>();
            //voiceActing = new List<string>;
            portraits = new List<Texture2D>();
            timers = new List<float>();
            currentLine = 0;

            singlePixel = TextureManager.GetTexure("singlePixel");
            avatarImage = TextureManager.GetTexure("cody01");
        }
        // add voiceActing string if you need
        public void AddLine(string line, Texture2D portrait, float timer)
        {
            
            // this.voiceActing.Add(voiceActing);   
            this.lines.Add(WrapText(line));
            this.portraits.Add(portrait);
            this.timers.Add(timer);

        }

        private string WrapText(String text) 
        {
            int maxLineWidth = 400; // Max width of text area
            string[] words = text.Split(' ');// Seperates text into words
            StringBuilder sb = new StringBuilder(); // Stitch words back together
            float lineWidth = 0f; // Set current width = 0, as we haven't started
            float spaceWidth = Game1.FontSmall.MeasureString(" ").X; /// How much space does a ''  take up
                                                                     /// 
            foreach (string word in words)
            {
                Vector2 size = Game1.FontSmall.MeasureString(word);

                // Make sure adding this word, will not go over lineWidth limit
                if (lineWidth + size.X < maxLineWidth)
                {
                    // add the word to final string
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    // Adding this word will take us over the max lineWidth
                    // Move to the next line
                    sb.Append(Environment.NewLine + word + " ");
                    lineWidth = size.X + spaceWidth; // Update lineWidth value
                }
            }
            
            // Send back final string
            return sb.ToString();

        }

        public void PlayFirstLine()
        {
            currentTime = timers[0];
            //SoundManager.PlayVoiceSound(voiceActing[0]);
            MusicManager.ChangeToVolume(MusicManager.VolumeTarget);
        }

        public void PlayScene(GameTime gT)
        {
            currentTime -= (float)gT.ElapsedGameTime.TotalSeconds;

            // This line has been showing itself for long enough
            if (currentTime <= 0)
                GoToNextLine();

            if (InputHelper.WasButtonPressed(PlayerIndex.One, Buttons.A) ||
                InputHelper.WasKeyPressed(Keys.Space))
            {
                //SoundManager.StopVoiceSound();
                GoToNextLine();
            }

        }

        private void GoToNextLine()
        {
            // Move to the next line
            currentLine++;

            // Has the current cutscene finsihed?
            if (currentLine > timers.Count - 1)
            {
                // We have finished the CutScene
                GameManager.Levels[GameManager.CurrentLevel].ActivateSceneEvent();
                // Play next cutscene
                GameManager.Levels[GameManager.CurrentLevel].CurrentCutScene++;
                return;
            }

            currentTime = this.timers[currentLine];
            //SoundManager.PlayVoiceSound(voiceActing[currentLine]);
        }

        public void Draw(SpriteBatch SB)
        {
            // Black background box
            SB.Draw(singlePixel, new Rectangle(100, 140, 400 + avatarImage.Width + 10,
                avatarImage.Height), new Color(Color.Black, 0.6f));

            // Draw Portrait
            SB.Draw(this.portraits[currentLine], new Vector2(100, 140), Color.White);

            // Draw Line text
            SB.DrawString(Game1.FontSmall, this.lines[currentLine], new Vector2(100 + avatarImage.Width + 10, 140), Color.White);

            // Draw Timer value
            //  SB.DrawString(Game1.FontSmall, "Time Remaining: " + currentTime.ToString(),
            //    new Vector2(100, 220), Color.White);


        }

    }
}
