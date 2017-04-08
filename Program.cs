using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistAppender
{
    class Program
    {

        readonly static string titlePrefix = "#EXTINF:";

        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to do Default update");
            Console.WriteLine("Otherwise specify the following items:");
            Console.WriteLine("The Location of the playlist to be updated:");
            string playList = Console.ReadLine();
            if (!playList.Equals(""))
            {

                Console.WriteLine("The location containing the new files");
                string filesToAddDirectory = Console.ReadLine();
                Console.WriteLine("The directory that the file will be put into");
                string directoryInFileList = Console.ReadLine();
                Console.WriteLine("The new file that the playlist will be written to. Leave blank to put it into a new folder named [updated]");
                string outputFileName = Console.ReadLine();
                Console.WriteLine("The section of the playlist to add it to (divided by [#EXTM3U])Leave blank if unsure");
                string sectionString = Console.ReadLine();
                if (sectionString.Equals(""))
                    sectionString = null;
                int section;
                ///section will be 0 if failed;
                Int32.TryParse(sectionString, out section);

                AppendAll(playList, filesToAddDirectory, directoryInFileList, section, outputFileName);
            }
            else
            {
                DefaultMyUpdate();
            }
            Console.WriteLine("Writing Files complete. Press enter to terminate");
             Console.ReadLine();
        }

        static void DefaultMyUpdate()
        {
            string addressPrefix = @"E:\Music\日本語\アニメ\";
            string addressPrefixExtra = addressPrefix + "[Extra]\\";
            string playList = @"E:\Music\日本語\Spring addition\[playlists]\";
            string playListMod = @"E:\Music\日本語\Spring addition\[playlists]\updated\";
            string[] playListFileNames = new string[] { "0.m3u", "アニメ.m3u", "日本語.m3u" };
            string filesLoc = @"E:\Music\日本語\Spring addition\[Anime]";
            string filesLocExtra = @"E:\Music\日本語\Spring addition\[Anime]\Extra";
            foreach (string playListFileName in playListFileNames)
            {
                Console.WriteLine("Appending to file: {0}" ,playListFileName);
                int files = AppendAll(playList + playListFileName, filesLoc, addressPrefix, 0);
                 files += AppendAll(playListMod + playListFileName, filesLocExtra, addressPrefixExtra, 1, playListMod + playListFileName);
                Console.Write("{0} files appended to playlist", files);
                Console.WriteLine("\n");
            }
        }

        private static int AppendAll(string listFile, string filesToAddDirectory,string addressPrefix, int sectionToAddTo)
        {
            return AppendAll(listFile, filesToAddDirectory, addressPrefix, sectionToAddTo, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listFile"></param>
        /// <param name="filesToAddDirectory"></param>
        /// <param name="sectionToAddTo"></param>
        /// <param name="outputFileName">Can be left null and will be completed to filename_new.extension</param>
        /// 
        private static int AppendAll(string listFile, string filesToAddDirectory, string addressPrefix, int sectionToAddTo, string outputFileName)
        {
            string[] original = System.IO.File.ReadAllLines(listFile);
            string[] toAdd = Directory.GetFiles(filesToAddDirectory);            

            List<string> originalList = new List<string>(original);

            List<int> offsets = new List<int>();
            for (int i =0; i<original.Length;i++)
            {
                if (original[i].Equals("#EXTM3U"))
                    offsets.Add(i);
            }

            Console.WriteLine("Added Files:");

            ///If I decide to do some chaining or whatenot in the future
            int addedLineCount = AddToSection(originalList, toAdd, addressPrefix, offsets[sectionToAddTo], offsets[sectionToAddTo+1]) - offsets[sectionToAddTo + 1];


            if (outputFileName == null)
            {
                outputFileName = listFile.Insert(listFile.LastIndexOf('\\')+1,"updated\\");
            }
            Directory.CreateDirectory(outputFileName.Substring(0,outputFileName.LastIndexOf('\\')));
            File.WriteAllLines(outputFileName, originalList);
            return addedLineCount / 2;
        }
        /// <summary>
        /// returns the new end position of the section 
        /// </summary>
        private static int AddToSection(List<string> playList, string[] filesToAdd, string addressPrefix, int sectionOffset, int sectionLimit)
        {
            if (sectionOffset == sectionLimit - 1)
            {
                sectionLimit = playList.Count;
            }
            foreach (string t in filesToAdd)
            {
                string fileName = t.Substring(t.LastIndexOf('\\') + 1);
                Track track = new Track(t);

                bool added = false;
                for (int i = sectionOffset; i < sectionLimit && !added; i++)
                {
                    string o = playList.ElementAt(i);
                    if (o.Contains("#EXT"))
                        continue;

                    if (o.Substring(o.LastIndexOf('\\') + 1).CompareTo(fileName) >= 0)
                    {

                        int ind = playList.IndexOf(o);
                        playList.Insert(ind - 1, titlePrefix + track);//#EXTINF:[duration],[artist] - [title]
                        playList.Insert(ind, addressPrefix + fileName);
                        sectionLimit += 2;
                        added = true;
                    }
                }
                if (!added)
                {
                    playList.Insert(sectionLimit, titlePrefix + track);
                    playList.Insert(sectionLimit + 1, addressPrefix + fileName);
                    sectionLimit += 2;
                    added = true;
                }
            }
            return sectionLimit;
        }
    }
}
