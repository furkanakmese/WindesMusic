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
        public List<Song> songPlaylist { get; set; }

        public int playlistID { get; set; }
        public string playlistName { get; set; }

        public Playlist()
        {
        }

        public void RefreshPlaylist()
        {
            songPlaylist = data.GetSongsInPlaylist(playlistID);
        }

        public List<Song> GetSongsInPlaylist()
        {
            return songPlaylist;
        }

        public void AddPlaylistSongToQueue(Song song)
        {
            MusicQueue.AddSongToQueue(song);
        }

        public void AddSongToPlaylist(Song song)
        {
            data.AddSongToPlaylist(this.playlistID, song.SongID);
            this.RefreshPlaylist();
        }

        public void AddSongToPlaylist(int SongId)
        {
            data.AddSongToPlaylist(this.playlistID, SongId);
            this.RefreshPlaylist();
        }

        public void DeleteSongFromPlaylist(int songId)
        {
            data.RemoveSongFromPlaylist(this.playlistID, songId);
        }

        public void DeletePlaylist()
        {
            data.DeletePlaylist(this.playlistID);
        }

        public void RenamePlaylist(string input)
        {
            playlistName = input;
            data.RenamePlaylist(this, input);
        }
    }
}
