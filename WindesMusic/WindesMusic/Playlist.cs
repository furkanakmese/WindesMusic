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

        public Recommender Recommender { get; set; }

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

        public void AddPlaylistSongToQueue(int SongID)
        {
            MusicQueue.AddSongToQueue(SongID);
        }

        public void AddSongToPlaylist(Song song)
        {
            data.AddSongToPlaylist(this.PlaylistID, song.SongID);
            this.RefreshPlaylist();
        }

        public void AddSongToPlaylist(int SongId)
        {
            data.AddSongToPlaylist(this.PlaylistID, SongId);
            this.RefreshPlaylist();
        }

        /*
        public void ShowPlaylistSongsPage(MainWindow main)
        {
            PlaylistSongsPage SongsPage = new PlaylistSongsPage(PlaylistID, PlaylistName, PlaylistSongs, main);
            main.Main.Content = SongsPage;
        }
        */
        public void CreatePlaylistFromQueue(string Name, int UserID)
        {
            if(MusicQueue.SongQueue != null)
            {
                //Queue<int> CopySongQueue = new Queue<int>(MQueue.SongQueue);
                data.CreateNewPlaylist(Name, UserID);
                //for(int i = 0; i < MQueue.SongQueue.Count(); i++)
                { 
                    //data.AddSongToPlaylist(CopySongQueue.Dequeue());
                }
            }
        }

        //Creates a queue with the songs in the order of the playlist
        public Queue<int> CreateQueueFromPlaylist()
        {
            List<int> SongList = new List<int>();
            Queue<int> SongQueue;
            foreach(Song PlaylistSong in SongPlaylist)
            {
                SongList.Add(PlaylistSong.SongID);
            }
            SongQueue = new Queue<int>(SongList);
            return SongQueue;
        }
    }
}
