using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public static class MusicQueue
    {
        public static Queue<Song> songQueue = new Queue<Song>();
        public static Queue<Song> recommendedSongQueue = new Queue<Song>();
        public static Stack<Song> previousSongs = new Stack<Song>();
        public static bool isShuffle = false;
        public static bool isRepeat = false;

        public static void AddSongToQueue(Song song)
        {
            songQueue.Enqueue(song);
        }

        public static void AddPlaylistToQueue(Playlist playlist, List<Song> RecommendedSongs)
        {
            //Adds all songs in the playlist to the queue
            foreach (Song song in playlist.songPlaylist)
            {
                songQueue.Enqueue(song);
            }
            //Adds the recommended songs to the seperate queue
            recommendedSongQueue.Clear();
            foreach(Song song in RecommendedSongs)
            {
                recommendedSongQueue.Enqueue(song);
            }
        }

        public static void AddPlaylistToQueue(Playlist playlist)
        {
            //Adds all songs in the playlist to the queue
            foreach (Song song in playlist.songPlaylist)
            {
                songQueue.Enqueue(song);
            }
        }

        public static void AddSongToPreviousQueue(Song song)
        {
            previousSongs.Push(song);
            //Removes the oldest placed song when the stack has more than ten songs
            if(previousSongs.Count > 10)
            {
                List<Song> _previousSongsList = new List<Song>(previousSongs);
                _previousSongsList.RemoveAt(9);
                previousSongs = new Stack<Song>(_previousSongsList);
            }
        }

        public static void ShuffleSongs()
        {
            List<Song> _songList = new List<Song>(songQueue);
            //Create a seed for the random, this seed makes every shuffle unique
            Random rng1 = new Random();
            int seed = rng1.Next(0, 1000);

            Random rng = new Random(seed);
            _songList = _songList.OrderBy(x => rng.Next()).ToList();
            songQueue = new Queue<Song>(_songList);
        }
    }
}
