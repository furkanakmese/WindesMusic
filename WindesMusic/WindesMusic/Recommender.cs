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
        public List<Song> getRecommendedSongsForPlaylist(Playlist playlist)
        {
            //Groups songs per genre
            var q = from x in playlist.SongPlaylist
                    group x by x.Genre into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };

            string mostCommonGenre = "";
            int highestPercentage = 0;

            //Determines what the most common genre is
            foreach (var x in q)
            {
                int percentage = (100 / playlist.SongPlaylist.Count) * x.Count;

                if (percentage > highestPercentage)
                {
                    mostCommonGenre = x.Value;
                    highestPercentage = percentage;
                }
            }
            foreach (Song song in db.GetRecommendedSongsForPlaylist(mostCommonGenre, playlist.PlaylistID))
            {
                Console.WriteLine(song.SongName);
            }
            return db.GetRecommendedSongsForPlaylist(mostCommonGenre, playlist.PlaylistID);
        }
    }
}