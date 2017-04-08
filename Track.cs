using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistAppender
{
    class Track
    {
        public int duration;
        public string artist;
        public string title;
        public string path;

        public Track(string title,string author,int length, string path)
        {
            this.title = title;
            this.artist = author;
            this.duration = length;
            this.path = path;
        }

        public Track()
        {
            duration = 0;
            artist = "";
            title = "";
            path = "";
        }

        override
        public string ToString()
        {
            return duration + "," + artist + " - " + title;
        }

        public Track (string fileName)
        {
            //Track track = new Track();
            ID3Info id3 = new ID3Info(fileName);
            if (fileName.Substring(fileName.LastIndexOf('.')).Equals(".mp3"))
            {
                id3.getMP3Data(this);
                this.duration = (int)id3.GetMPDuration();
                Console.WriteLine(this);
            }
            else if (fileName.Substring(fileName.LastIndexOf('.')).Equals(".flac"))
            {
                id3.getFlacDataAndDuration(this);
                Console.WriteLine(this);
            }
        }

    }
}
