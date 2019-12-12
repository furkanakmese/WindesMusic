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
        public List<Song> GetRecommendedSongsForPlaylist(Playlist playlist, int amount)
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

            return db.GetRecommendedSongsForPlaylist(mostCommonGenre, secondMostCommonGenre, playlist.PlaylistID, amount);
        }

        //Returns a maximum of 10 previously listened to songs
        public Playlist getDailyPlaylist(int userID, string playlistName)
        {
            var generatedID = db.SaveGeneratedPlaylist(userID, playlistName);
            Playlist playlist = db.GetHistoryPlaylist(userID, playlistName);

            //Creates the playlist if it hasn't been created yet today
            if (generatedID != null)
            {
                if(playlistName == "History Playlist")
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
                else if(playlistName == "Daily Playlist")
                {
                    playlist.SongPlaylist = db.GetPlayHistory(userID);
                    playlist.SongPlaylist = GetRecommendedSongsForPlaylist(playlist, 10);
                    db.SaveGeneratedPlaylistToSong(playlist.SongPlaylist, (int)generatedID);
                    return playlist;
                }
                
            }

            playlist.SongPlaylist = db.getGeneratedPlaylistSongs(playlist.PlaylistID);
            return playlist;
        }

        public List<Song> GetRecommendedAdsFromPlaylist(Playlist playlist)
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
            List<Song> test = db.GetRecommendedAdsForPlaylist(mostCommonGenre, secondMostCommonGenre, playlist.PlaylistID);
            foreach (var item in test) Console.WriteLine(item.SongName);
            return db.GetRecommendedAdsForPlaylist(mostCommonGenre, secondMostCommonGenre, playlist.PlaylistID);
        }
    }
}