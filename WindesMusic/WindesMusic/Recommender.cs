using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public class Recommender
    {

        public Recommender()
        {

        }

        public void getRecommendedSongsForPlaylist(List<Song> playlistSongs, Database db)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < playlistSongs.Count; i++)
            {
                if(i < playlistSongs.Count - 1)
                {
                    builder.Append(playlistSongs.ElementAt(i).SongID).Append(" AND SongID != ");
                }
                else
                {
                    builder.Append(playlistSongs.ElementAt(i).SongID);
                }
                
            }

            Console.WriteLine(builder);

            var q = from x in playlistSongs
                    group x by x.Genre into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };

            foreach (var x in q)
            {
                //Console.WriteLine($"Genre: {x.Value} Percentage: {(100 / playlistSongs.Count) * x.Count}");
                int percentage = (100 / playlistSongs.Count) * x.Count;
                
                
            }
            foreach (Song song in db.GetRecommendedSongsForPlaylist(builder.ToString()))
            {
                Console.WriteLine(song.SongName);
            }


        }
    }
}
