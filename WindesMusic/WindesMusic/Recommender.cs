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
            //Groups songs per genre and orders them by count
            var q = from x in playlist.SongPlaylist
                    group x by x.Subgenre into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };

            string mostCommonGenre = "";
            string secondMostCommonGenre = "";
            int loopCount = 0;
            //Determines what the most common and second most common genre is
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

            return db.GetRecommendedSongsForPlaylist(mostCommonGenre, secondMostCommonGenre, playlist.PlaylistID);
        }

        //Returns a maximum of 10 previously listened to songs
        public Playlist getHistoryPlaylist(int userID)
        {
            var generatedID = db.SaveGeneratedPlaylist("History Playlist", userID);
            Playlist playlist = db.GetHistoryPlaylist(userID);

            //Creates the playlist if it hasn't been created yet today
            if (generatedID != null)
            {
                playlist.SongPlaylist = db.GetPlayHistory(userID);

                Random random = new Random();
                var randomizedList = from song in playlist.SongPlaylist
                                     orderby random.Next()
                                     select song;

                List<Song> history = new List<Song>();
                int loopCount = 0;
                foreach (var song in randomizedList)
                {
                    if (loopCount < 10)
                    {
                        history.Add(song);
                        loopCount++;
                    }
                }
                playlist.SongPlaylist = history;

                db.SaveGeneratedPlaylistToSong(history, (int)generatedID);
                return playlist;
            }

            playlist.SongPlaylist = db.getGeneratedPlaylistSongs(playlist.PlaylistID);

            return playlist;
        }
    }
}