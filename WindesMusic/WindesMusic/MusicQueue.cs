using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public class MusicQueue
    {
        public static Queue<int> SongQueue = new Queue<int>();
        public static Stack<int> PreviousSongs = new Stack<int>();

        public MusicQueue()
        {
            
        }

        public void AddSongToQueue(int SongID)
        {
            SongQueue.Enqueue(SongID);
        }

        public void AddSongToPreviousQueue(int SongID)
        {
            PreviousSongs.Push(SongID);
            if(PreviousSongs.Count > 10)
            {
                List<int> PreviousSongsList = new List<int>(PreviousSongs);
                PreviousSongsList.RemoveAt(9);
                PreviousSongs = new Stack<int>(PreviousSongsList);
            }
        }

        public void RemoveSongFromQueue(int Key)
        {
            List<int> SongList = new List<int>(SongQueue);
            SongList.RemoveAt(Key);
            SongQueue = new Queue<int>(SongList);
        }

        public int GetSongFromQueue()
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

        public List<int> ReturnSongsInQueue()
        {
            List<int> ReturnList = new List<int>(SongQueue);
            return ReturnList;
        }
    }
}
