using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public class Recommender
    {
        private Database db;

        public Recommender(Database db)
        {
            this.db = db;
        }
        //Returns 5 random songs that are not in the playlist and with the most common genre
        public List<Song> GetRecommendedSongsForPlaylist(Playlist playlist)
        {
            //Groups songs per genre orders by count
            var q = from x in playlist.SongPlaylist
                    group x by x.Subgenre into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };

            string mostCommonGenre = "";
            string secondMostCommonGenre = "";
            int loopCount = 0;
            //Determines what the most common genre is
            foreach (var x in q)
            {
                if (loopCount == 0)
                {
                    mostCommonGenre = x.Value;
                }
                else if (loopCount == 1)
                {
                    secondMostCommonGenre = x.Value;
                }
                loopCount++;
            }

            foreach (Song song in db.GetRecommendedSongsForPlaylist(mostCommonGenre, secondMostCommonGenre, playlist.PlaylistID))
            {
                Console.WriteLine(song.SongName);
            }

            return db.GetRecommendedSongsForPlaylist(mostCommonGenre, secondMostCommonGenre, playlist.PlaylistID);
        }
    }
}