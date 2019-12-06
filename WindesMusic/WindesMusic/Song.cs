using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public class Song
    {
        public int SongID { get; set; }
        public string SongName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public string Subgenre { get; set; }
        public int UserID { get; set; }
    }
}
