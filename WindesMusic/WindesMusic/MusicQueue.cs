using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public static class MusicQueue
    {
        public static Queue<int> SongQueue = new Queue<int>();
        public static Stack<int> PreviousSongs = new Stack<int>();


        public static void AddSongToQueue(int SongID)
        {
            SongQueue.Enqueue(SongID);
        }

        public static void AddSongToPreviousQueue(int SongID)
        {
            PreviousSongs.Push(SongID);
            if(PreviousSongs.Count > 10)
            {
                List<int> PreviousSongsList = new List<int>(PreviousSongs);
                PreviousSongsList.RemoveAt(9);
                PreviousSongs = new Stack<int>(PreviousSongsList);
            }
        }

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

        public static void RemoveSongFromQueue(int Key)
        {
            List<int> SongList = new List<int>(SongQueue);
            SongList.RemoveAt(Key);
            SongQueue = new Queue<int>(SongList);
        }

        public static int GetSongFromQueue()
        {
            if (SongQueue.Count != 0)
            {
                int SongID = SongQueue.Dequeue();
                return SongID;
            }
            else
            {
                return 23; //Temporary, change it later to start playing recommendations
            }
        }

        public static List<int> ReturnSongsInQueue()
        {
            List<int> ReturnList = new List<int>(SongQueue);
            return ReturnList;
        }
    }
}
