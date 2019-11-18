using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public class Playlist
    {
        Database data = new Database();
        public List<int> SongPlaylist;
        public int PlaylistID;
        public string Name;
        public int UserID;

        public Playlist(int? SongPlaylistID)
        {
            int TemporaryPlaylistID = 0;
            //Sets TemporaryPlaylistID to the given number if the number isn't null
            if (SongPlaylistID != null)
            {
                TemporaryPlaylistID = SongPlaylistID.GetValueOrDefault();
            }
            //Creates a random number and checks the database if it's already in use, if it is in use, the loop starts again and a new number is generated
            else
            {
                Random rnd = new Random();
                bool TestID = true;
                while (TestID == true)
                {

                    TemporaryPlaylistID = rnd.Next(0, 9999);
                    if(data.GetRecordsInt($"SELECT PlaylistID FROM Playlist WHERE PlaylistID = {TemporaryPlaylistID}", "PlaylistID") == null)
                    {
                        TestID = false;
                    }
                }
            }
            PlaylistID = TemporaryPlaylistID;
            SongPlaylist = data.GetRecordsInt($"SELECT SongID FROM Song WHERE SongID IN (SELECT SongID FROM PlaylistToSong WHERE PlaylistID = {SongPlaylistID})", "SongID");
        }

        public void CreateNewPlaylist(string Name, int UserID)
        {
            data.SetValues($"INSERT INTO Playlist VALUES({PlaylistID}, '{Name}', {UserID})");
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

        public void AddSongToPlaylist(int DesiredSong)
        {
            data.SetValues($"INSERT INTO PlaylistToSong(PlaylistID, SongID) VALUES ({PlaylistID}, {DesiredSong})");
        }

        public void RemoveSongFromPlaylist(int SongToDelete)
        {
            data.SetValues($"DELETE FROM PlaylistToSong WHERE PlaylistID = {PlaylistID} AND SongID = {SongToDelete}");
        }

        public void RefreshPlaylist()
        {
            SongPlaylist = data.GetRecordsInt($"SELECT SongID FROM Song WHERE SongID IN (SELECT SongID FROM PlaylistToSong WHERE PlaylistID = {PlaylistID})", "SongID");
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
            foreach (int SongID in SongPlaylist)
            {
                SongQueue.AddSongToQueue(SongID);
            }
            return SongQueue;
        }
    }
}
