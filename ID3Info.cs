using FlacLibSharp;
using Id3;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistAppender
{
    class ID3Info
    {
        private string MediaFilename;
        public ID3Info(string MediaFilename)
        {
            this.MediaFilename = MediaFilename;
        }
        public void  ID3InfoTest(string file)
        {
            byte[] b = new byte[128];
            string sTitle;
            string sSinger;
            string sAlbum;
            string sYear;
            string sComm;

            FileStream fs = new FileStream(file, FileMode.Open);
            fs.Seek(-128, SeekOrigin.End);
            fs.Read(b, 0, 128);
            bool isSet = false;
            String sFlag = System.Text.Encoding.Default.GetString(b, 0, 3);
            if (sFlag.CompareTo("TAG") == 0)
            {
                System.Console.WriteLine("Tag   is   setted! ");
                isSet = true;
            }

            if (isSet)
            {
                //get   title   of   song; 
                sTitle = System.Text.Encoding.Default.GetString(b, 3, 30);
                System.Console.WriteLine("Title: " + sTitle);
                //get   singer; 
                sSinger = System.Text.Encoding.Default.GetString(b, 33, 30);
                System.Console.WriteLine("Singer: " + sSinger);
                //get   album; 
                sAlbum = System.Text.Encoding.Default.GetString(b, 63, 30);
                System.Console.WriteLine("Album: " + sAlbum);
                //get   Year   of   publish; 
                sYear = System.Text.Encoding.Default.GetString(b, 93, 4);
                System.Console.WriteLine("Year: " + sYear);
                //get   Comment; 
                sComm = System.Text.Encoding.Default.GetString(b, 97, 30);
                System.Console.WriteLine("Comment: " + sComm);
            }
            fs.Close();
            System.Console.WriteLine("Any   key   to   exit! ");
            System.Console.Read();
        }

        /// <summary>
        /// http://id3.codeplex.com/
        /// </summary>
        /// <param name="track"></param>
        public void getMP3Data(Track track)
        {
            var fs = new FileStream(MediaFilename, FileMode.Open, FileAccess.Read);
            var mp3 = new Mp3Stream(fs);
            
           Id3Tag tag = mp3.GetTag(Id3TagFamily.FileStartTag);
           track.title= tag.Title.Value;
           track.artist = tag.Artists.Value;

            fs.Close();
            
            
        }

        public void getMP3DataDirty(Track track)
        {
            byte[] b = new byte[67];      
            
            FileStream fs = new FileStream(MediaFilename, FileMode.Open);
            fs.Seek(-128, SeekOrigin.End);
            fs.Read(b, 0, 67);
            bool isSet = false;
            String sFlag = System.Text.Encoding.Default.GetString(b, 0, 3);
            if (sFlag.CompareTo("TAG") == 0)
            {
                isSet = true;
            }

            if (isSet)
            {
                //get   title   of   song; 
                track.title = System.Text.Encoding.Default.GetString(b, 3, 30);
                int end = track.title.IndexOf('\0');

                track.title = track.title.Substring(0,end!=-1? end :29);
                //get   singer; 
                track.artist = System.Text.Encoding.Default.GetString(b, 33, 30);
                end = track.artist.IndexOf('\0');
                track.artist = track.artist.Substring(0, end != -1 ? end : 29);
            fs.Close();
        }
    }

       public double GetMPDuration()
        {
            double duration = 0.0;
            using (FileStream fs = File.OpenRead(MediaFilename))
            {
                Mp3Frame frame = Mp3Frame.LoadFromStream(fs);
                if (frame != null)
                {
                    uint _sampleFrequency = (uint)frame.SampleRate;
                }
                while (frame != null)
                {
                    if (frame.ChannelMode == ChannelMode.Mono)
                    {
                        duration += (double)frame.SampleCount * 2.0 / (double)frame.SampleRate;
                    }
                    else
                    {
                        duration += (double)frame.SampleCount / (double)frame.SampleRate;
                    }
                    frame = Mp3Frame.LoadFromStream(fs);
                }
            }
            return duration;
        }

        /// <summary>
        /// https://github.com/AaronLenoir/flaclibsharp
        /// </summary>
        /// <param name="track"></param>
        public void getFlacDataAndDuration(Track track)
        {
            using (FlacFile file = new FlacFile(MediaFilename))
            {
                // Access to the StreamInfo class (actually this should ALWAYS be there ...)
                var streamInfo = file.StreamInfo;
                if (streamInfo != null)
                {
                    track.duration = streamInfo.Duration;
                }

                // Access to the VorbisComment IF it exists in the file
                var vorbisComment = file.VorbisComment;
                if (vorbisComment != null)
                {
                    track.artist = vorbisComment.Artist.Value;
                    track.title = vorbisComment.Title.Value;
                }
                
            }
        }
    }
}
