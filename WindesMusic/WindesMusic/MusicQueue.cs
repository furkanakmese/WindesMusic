using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public static class MusicQueue
    {
        public static Queue<Song> SongQueue = new Queue<Song>();
        public static Queue<Song> RecommendedSongQueue = new Queue<Song>();
        public static Stack<Song> PreviousSongs = new Stack<Song>();
        public static bool IsShuffle = false;

        public static void AddSongToQueue(Song song)
        {
            SongQueue.Enqueue(song);
        }

        public static void AddPlaylistToQueue(Playlist playlist, List<Song> RecommendedSongs)
        {
            foreach (Song song in playlist.SongPlaylist)
            {
                SongQueue.Enqueue(song);
            }
            RecommendedSongQueue.Clear();
            foreach(Song song in RecommendedSongs)
            {
                RecommendedSongQueue.Enqueue(song);
            }
        }

        public static void AddPlaylistToQueue(Playlist playlist)
        {
            foreach (Song song in playlist.SongPlaylist)
            {
                SongQueue.Enqueue(song);
            }
        }

        public static void AddSongToPreviousQueue(Song song)
        {
            PreviousSongs.Push(song);
            if(PreviousSongs.Count > 10)
            {
                List<Song> PreviousSongsList = new List<Song>(PreviousSongs);
                PreviousSongsList.RemoveAt(9);
                PreviousSongs = new Stack<Song>(PreviousSongsList);
            }
        }
        
        public static void RemoveSongFromQueue(int Key)
        {
            List<Song> SongList = new List<Song>(SongQueue);
            SongList.RemoveAt(Key);
            SongQueue = new Queue<Song>(SongList);
        }

        public static Song GetSongFromQueue()
        {
            if (SongQueue.Count != 0)
            {
                Song song = SongQueue.Dequeue();
                return song;
            }
            else
            {
                return null; //Temporary, change it later to start playing recommendations
            }
        }

        public static List<Song> ReturnSongsInQueue()
        {
            List<Song> ReturnList = new List<Song>(SongQueue);
            return ReturnList;
        }

        public static void ShuffleSongs()
        {
            List<Song> SongList = new List<Song>(SongQueue);
            //Create a seed for the random, this seed makes every shuffle unique
            Random rng1 = new Random();
            int seed = rng1.Next(0, 1000);

            Random rng = new Random(seed);
            SongList = SongList.OrderBy(x => rng.Next()).ToList();
            SongQueue = new Queue<Song>(SongList);
        }
    }
}
