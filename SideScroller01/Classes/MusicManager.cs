using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SideScroller01
{
    enum MusicState
    {
        Fading,
        Rising,
        Holding
    }

    static class MusicManager
    {
        private static string rootPath;
        private static Dictionary<string, Song> songs;
        
        public static MusicState State = MusicState.Holding;
        public static float CurrentVolume = 0.75f;
        public static float VolumeTarget = 0.02f;
        public static float RateOfChange = 0.0008f;

        public static void Initialize(ContentManager content, string path = @"\Music")
        {
            songs = new Dictionary<string, Song>();

            rootPath = path;
            LoadContent(content);
        }

        private static void LoadContent(ContentManager Content)
        {
            Debug.WriteLine("Content: " + Content);

            string soundKey;
            int fileLength;
            int fileExtLength = 4;      // .wav, .jpg, etc...

            DirectoryInfo rootDirectory = new DirectoryInfo(Content.RootDirectory + rootPath);

            // first scan all subdirectories and load files
            DirectoryInfo[] subFolders = rootDirectory.GetDirectories("*");
            foreach (DirectoryInfo subFolder in subFolders)
            {
                // Debug.WriteLine("subFolder.Name: " + subFolder.Name);
          
                // get folder info and grab file info in it
                DirectoryInfo rootSubFolderInfo = new DirectoryInfo(Content.RootDirectory + rootPath + @"\" + subFolder.Name);
                FileInfo[] subFiles = rootSubFolderInfo.GetFiles("*.xnb");

                // scan the sub files and load
                foreach (FileInfo subFileItem in subFiles)
                {
                    fileLength = subFileItem.Name.Length;
                    soundKey = subFileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension

                    string subFilePath = rootDirectory.Name + @"\" + rootSubFolderInfo.Name + @"\";
                    subFilePath += soundKey;

                    // add sound to our effects dictionary
                    songs.Add(soundKey, Content.Load<Song>(subFilePath));

                    // Debug.WriteLine("soundKey: " + soundKey);
                    // Debug.WriteLine("subFilePath: " + subFilePath);
                }
            }

            // get all files that are only within the root directory
            FileInfo[] Files = rootDirectory.GetFiles("*.xnb");
            Debug.WriteLine("Files.Length: " + Files.Length);
            foreach (FileInfo fileItem in Files)
            {
                fileLength = fileItem.Name.Length;
                soundKey = fileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension
                string filePath = rootDirectory.Name + @"\" + soundKey;

                // add sound to our effects dictionary
                songs.Add(soundKey, Content.Load<Song>(filePath));

                //Debug.WriteLine("songs: " + songs);
                //Debug.WriteLine("soundKey: " + soundKey);
                //Debug.WriteLine("filePath: " + filePath);
            }
        }

        public static void Update()
        {
            switch (State)
            {
                case MusicState.Rising:
                    if (MediaPlayer.Volume < VolumeTarget)
                    {
                        MediaPlayer.Volume += RateOfChange;
                        if (MediaPlayer.Volume > VolumeTarget)
                        {
                            MediaPlayer.Volume = VolumeTarget;
                            State = MusicState.Holding;
                        }
                    }
                    break;

                case MusicState.Fading:
                    if (MediaPlayer.Volume > VolumeTarget)
                    {
                        MediaPlayer.Volume -= RateOfChange;
                        if (MediaPlayer.Volume < VolumeTarget)
                        {
                            MediaPlayer.Volume = VolumeTarget;
                            State = MusicState.Holding;
                        }
                    }
                    break;
            }

        }

        public static void ChangeToVolume(float setTo)
        {
            VolumeTarget = setTo;

            if (VolumeTarget < MediaPlayer.Volume)
                State = MusicState.Fading;
            else
                State = MusicState.Rising;
        }

        public static void PlaySong(string songKey)
        {
            Song instance = songs[songKey];

            MediaPlayer.Play(instance);
            MediaPlayer.Volume = CurrentVolume;
        }

        public static void SetRepeating(bool IsRepeating)
        {
            MediaPlayer.IsRepeating = IsRepeating;
        }
       
        public static void StopSong()
        {
            MediaPlayer.Stop();

        }
    }
}
