using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public class Playlist
    {
        private Database data = new Database();
        public List<Song> SongPlaylist { get; set; }

        public int PlaylistID { get; set; }
        public string PlaylistName { get; set; }

        public Playlist()
        {
        }

        public Playlist(int PlaylistId)
        {
            PlaylistID = PlaylistId;
        }

        public void RefreshPlaylist()
        {
            SongPlaylist = data.GetSongsInPlaylist(PlaylistID);
        }

        public void AddPlaylistSongToQueue(MusicQueue MQueue, int SongID)
        {
            if(MQueue == null)
            {
                MusicQueue SongQueue = new MusicQueue();
                SongQueue.AddSongToQueue(SongID);
            }
            else
            {
                MQueue.AddSongToQueue(SongID);
            }
        }

        public void CreatePlaylistFromQueue(MusicQueue MQueue, string Name, int UserID)
        {
            if(MQueue != null)
            {
                Queue<int> CopySongQueue = new Queue<int>(MQueue.SongQueue);
                data.CreateNewPlaylist(Name, UserID);
                for(int i = 0; i < MQueue.SongQueue.Count(); i++)
                { 
                    //data.AddSongToPlaylist(CopySongQueue.Dequeue());
                }
            }
        }

        //Creates a queue with the songs in the order of the playlist
        public MusicQueue CreateQueueFromPlaylist()
        {
            MusicQueue SongQueue = new MusicQueue();
            foreach(Song PlaylistSong in SongPlaylist)
            {
                SongQueue.AddSongToQueue(PlaylistSong.SongID);
            }        
            return SongQueue;
        }
    }
}
