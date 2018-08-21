using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace SideScroller01
{
    static class TextureManager
    {
        private static string rootPath;
        private static ContentManager contentManager;
        private static Dictionary<string, Texture2D> textures;

        public static void Initialize(ContentManager content, string path = @"Textures")
        {
            textures = new Dictionary<string, Texture2D>();

            rootPath = path;
            contentManager = content;

            // LoadContent(content);

            Console.WriteLine("Initialize: RootDirectory: '{0}' rootPath: '{1}'", contentManager.RootDirectory, rootPath);
            DirectoryInfo rootDirectory = new DirectoryInfo(contentManager.RootDirectory + rootPath);
            ProcessDirectory(contentManager.RootDirectory, rootPath);
        }

        private static void ProcessDirectory(string rootPath, string targetRoot, string targetName = "", string targetFull = "")
        {
            Console.WriteLine("ProcessDirectory: rootPath: '{0}' ", rootPath);
            Console.WriteLine("ProcessDirectory: targetRoot: '{0}' targetName: '{1}'", targetRoot, targetName);

            string textureKey;
            int fileLength;
            int fileExtLength = 4;      // .png, .jpg, etc...

            DirectoryInfo rootDirectory = new DirectoryInfo(rootPath + @"\" + targetRoot + @"\" + targetName);
            Console.WriteLine("rootDirectory: Name: '{0}'", rootDirectory.Name);

            FileInfo[] Files = rootDirectory.GetFiles("*");
            foreach (FileInfo fileItem in Files)
            {
                Console.WriteLine("File: Name: '{0}' Extension: '{1}'", fileItem.Name, fileItem.Extension);

                fileLength = fileItem.Name.Length;
                textureKey = fileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension
                // string filePath = rootDirectory.Name + @"\" + textureKey;
                string filePath;
                
                if (targetName == "") filePath = targetRoot + @"\" + textureKey;
                else filePath = targetRoot + @"\" + targetName + @"\" + textureKey;

                ProcessFile(textureKey, filePath, fileItem.Extension);
            }

            // first scan all subdirectories and load files
            DirectoryInfo[] subFolders = rootDirectory.GetDirectories("*");

            targetRoot += @"\" + targetName;

            foreach (DirectoryInfo subFolder in subFolders)
            {
                string subFolderName = subFolder.Name;
                // string targetSubFolderName = targetDirectory + @"\" + subFolderName;
                // string targetSubFolderName = targetRoot + @"\" + targetName;

                Console.WriteLine("Directory: rootPath: '{0}' targetSubFolderName: '{1}'", rootPath, targetRoot);
                Console.WriteLine("Directory: subFolderName: '{0}'", subFolderName);

                ProcessDirectory(rootPath, targetRoot, subFolderName);
            }

        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string textureKey, string path, string extension)
        {
            Console.WriteLine("Processed file: Path: '{0}' TextureKey: '{1}'", path, textureKey);

            // add texture to our dictionary
            if (!textures.ContainsKey(textureKey))
                textures.Add(textureKey, contentManager.Load<Texture2D>(path));
        }

        private static void LoadContent(ContentManager Content)
        {
            string textureKey;
            int fileLength;
            int fileExtLength = 4;      // .png, .jpg, etc...

            DirectoryInfo rootDirectory = new DirectoryInfo(Content.RootDirectory + rootPath);

            // we will be workingn 2 sub levels

            // first scan all subdirectories and load files
            DirectoryInfo[] subFolders = rootDirectory.GetDirectories("*");
            foreach (DirectoryInfo subFolder in subFolders)
            {
                Debug.WriteLine("subFolder.Name: " + subFolder.Name);

                string subFolderName = subFolder.Name;

                /* get folder info and grab file info in it
                DirectoryInfo rootSubFolderInfo = new DirectoryInfo(Content.RootDirectory + rootPath + @"\" + subFolder.Name);
                FileInfo[] subFiles = rootSubFolderInfo.GetFiles("*");

                // scan the sub files and load
                foreach (FileInfo subFileItem in subFiles)
                {
                    fileLength = subFileItem.Name.Length;
                    textureKey = subFileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension

                    string subFilePath = rootDirectory.Name + @"\" + rootSubFolderInfo.Name + @"\";
                    subFilePath += textureKey;

                    // add sound to our effects dictionary
                    textures.Add(textureKey, Content.Load<Texture2D>(subFilePath));
                }*/
            }

            /* get all files that are only within the root directory
            FileInfo[] Files = rootDirectory.GetFiles("*");
            foreach (FileInfo fileItem in Files)
            {
                fileLength = fileItem.Name.Length;
                textureKey = fileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension
                string filePath = rootDirectory.Name + @"\" + textureKey;

                // add sound to our effects dictionary
                textures.Add(textureKey, Content.Load<Texture2D>(filePath));
            }*/
        }

        private static void ScanFolder()
        {
            /* first scan all subdirectories and load files
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
                    textureKey = subFileItem.Name.Substring(0, fileLength - fileExtLength);   // remove extension

                    string subFilePath = rootDirectory.Name + @"\" + rootSubFolderInfo.Name + @"\";
                    subFilePath += textureKey;

                    // add sound to our effects dictionary
                    textures.Add(textureKey, Content.Load<Texture2D>(subFilePath));
                }
            }*/


        }

        public static Texture2D GetTexure(string textureName)
        {
            return textures[textureName];
        }

        /*public static SpriteFont GetSpriteFont(string fontName)
        {
            return textures[fontName];
        }*/

    }
}
