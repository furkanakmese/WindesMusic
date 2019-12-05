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
        public static Stack<Song> PreviousSongs = new Stack<Song>();


        public static void AddSongToQueue(Song song)
        {
            SongQueue.Enqueue(song);
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

        /*
        public static void AddSongToPreviousQueue(string SongString)
        {
            PreviousSongs.Push(Convert.ToInt32(SongString));
            if (PreviousSongs.Count > 10)
            {
                List<int> PreviousSongsList = new List<int>(PreviousSongs);
                PreviousSongsList.RemoveAt(9);
                PreviousSongs = new Stack<int>(PreviousSongsList);
            }
        }
        */

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
    }
}
