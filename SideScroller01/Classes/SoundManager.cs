using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SideScroller01
{
    static class SoundManager
    {
        public static float CurrentVolume = 0.95f;

        private static string rootPath;
        private static Dictionary<string, SoundEffect> soundEffects;

        public static void Initialize(ContentManager content, string path = @"\Sounds")
        {
            soundEffects = new Dictionary<string, SoundEffect>();

            rootPath = path;
            LoadContent(content);
        }

        private static void LoadContent(ContentManager Content)
        {
            string soundKey;
            int fileLength;
            int fileExtLength = 4;      // .wav, .jpg, etc...

            DirectoryInfo rootDirectory = new DirectoryInfo(Content.RootDirectory + rootPath);

            // first scan all subdirectories and load files
            DirectoryInfo[] subFolders = rootDirectory.GetDirectories("*");
            foreach (DirectoryInfo subFolder in subFolders)
            {
                // get folder info and grab file info in it
                DirectoryInfo rootSubFolderInfo = new DirectoryInfo(Content.RootDirectory + rootPath + @"\" + subFolder.Name);
                FileInfo[] subFiles = rootSubFolderInfo.GetFiles("*");

                // scan the sub files and load
                foreach (FileInfo subFileItem in subFiles)
                {
                    fileLength = subFileItem.Name.Length;
                    soundKey = subFileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension
                    
                    string subFilePath = rootDirectory.Name + @"\" + rootSubFolderInfo.Name + @"\";
                    subFilePath += soundKey;  
                    
                    // add sound to our effects dictionary
                    soundEffects.Add(soundKey, Content.Load<SoundEffect>(subFilePath));
                }
            }

            // get all files that are only within the root directory
            FileInfo[] Files = rootDirectory.GetFiles("*");
            foreach (FileInfo fileItem in Files)
            {
                fileLength = fileItem.Name.Length;
                soundKey = fileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension
                string filePath = rootDirectory.Name + @"\" + soundKey;
      
                // add sound to our effects dictionary
                soundEffects.Add(soundKey, Content.Load<SoundEffect>(filePath));
            }
        }

        public static void PlaySound(string soundName, bool useRandomPitch = false)
        {
            // when we play a sound, we will create an instance of it
            SoundEffectInstance instance = soundEffects[soundName].CreateInstance();
            instance.Volume = CurrentVolume;

            // for some sound, we want to adjust its pitch randomly
            if (useRandomPitch)
            {
                float randomPitchRange = ((float)Game1.Random.NextDouble() * 0.15f) - 0.1f;
                // Debug.WriteLine("RAND: {0}", randomPitchRange);

                instance.Pitch = randomPitchRange;// 0.25f * (float)Game1.Random.NextDouble();
            }

            instance.Play();
        }

        public static void PlayVoiceSound(string soundName)
        {
            // when we play a voice sound, we use the original
            soundEffects[soundName].Play();
        }
    }
}
