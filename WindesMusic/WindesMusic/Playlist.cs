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
        

        public void CreateNewPlaylist(string Name, int UserID)
        {
            data.SetValues($"INSERT INTO Playlist(PlaylistName, UserID) VALUES('{Name}', {UserID})");
        }

        public void ChangePlaylistName(string Name)
        {
            data.SetValues($"UPDATE Playlist SET Name = '{Name}' WHERE PlaylistID = {PlaylistID}");
        }

        public void DeletePlaylist()
        {
            data.SetValues($"DELETE FROM PlaylistToSong WHERE PlaylistID = {PlaylistID}");
            data.SetValues($"DELETE FROM Playlist WHERE PlaylistID = {PlaylistID}");
        }

        public void AddSongToPlaylist(int DesiredSong)//rename DesiredSong to SongID?
        {
            data.SetValues($"INSERT INTO PlaylistToSong(PlaylistID, SongID) VALUES ({PlaylistID}, {DesiredSong})");
        }

        public void RemoveSongFromPlaylist(int SongToDelete)
        {
            data.SetValues($"DELETE FROM PlaylistToSong WHERE PlaylistID = {PlaylistID} AND SongID = {SongToDelete}");
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
                this.CreateNewPlaylist(Name, UserID);
                for(int i = 0; i < MQueue.SongQueue.Count(); i++)
                { 
                    this.AddSongToPlaylist(CopySongQueue.Dequeue());
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
