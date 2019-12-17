using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using WindesMusic;
using System.Linq;

namespace UnitTestWindesMusic
{
    [TestClass]
    class MusicManagementTests
    {

        [TestMethod]
        public void TestAddSongToPlaylist()
        {
            //Arrange
            //Get playlist from Database
            Database db = new Database();
            User user = new User();
            user = db.GetUserData(1);
            Playlist playlist = user.Playlists.Where(i => i.PlaylistID == 2).FirstOrDefault();
            Song song = playlist.SongPlaylist[1];
            int AmountOfSongs = playlist.SongPlaylist.Count();

            //Act
            //Add the song to the playlist and refresh the playlist
            playlist.AddSongToPlaylist(song);
            playlist.RefreshPlaylist();

            //Assert
            //Check if the amount of songs has increased by one
            int NewAmountOfSongs = playlist.SongPlaylist.Count();
            Assert.IsTrue(NewAmountOfSongs == (AmountOfSongs + 1));
        }

        [TestMethod]
        public void TestAddCorrectSongToPlaylist()
        {
            //Arrange
            //Get playlist from Database
            Database db = new Database();
            User user = new User();
            user = db.GetUserData(1);
            Playlist playlist = user.Playlists.Where(i => i.PlaylistID == 2).FirstOrDefault();
            Song song = playlist.SongPlaylist[1];

            //Act
            //Add the song to the playlist and refresh the playlist
            playlist.AddSongToPlaylist(song);
            playlist.RefreshPlaylist();

            //Assert
            //Check if the amount of songs has increased by one
            Song song1 = new Song();
            song1 = playlist.SongPlaylist.Last();
            Assert.IsTrue(song.SongID == song1.SongID);
        }

        [TestMethod]
        public void TestCreatePlaylist()
        {
            //Arrange
            //Get Userdata
            Database db = new Database();
            User user = new User();
            user = db.GetUserData(1);
            int AmountOfPlaylists = user.Playlists.Count();
            //Act
            //Create new playlist with a name
            db.CreateNewPlaylist("TestPlaylist", 1);

            //Assert
            //Check if the new playlist has been added by checking the amount of playlists the user has
            user = db.GetUserData(1);
            int NewAmountOfPlaylists = user.Playlists.Count();
            Assert.IsTrue(NewAmountOfPlaylists == (AmountOfPlaylists + 1));
        }

        [TestMethod]
        public void TestChangePlaylistName()
        {
            //Arrange
            //Get Userdata
            Database db = new Database();
            User user = new User();
            user = db.GetUserData(1);
            List<Playlist> playlists = user.Playlists;
            Playlist playlist1 = playlists[1];

            //Act
            //Change playlistname of playlist
            db.RenamePlaylist(playlist1, "Great songs");

            //Assert
            //Check if the name has been changed
            user = db.GetUserData(1);
            Playlist playlist = user.Playlists.Where(i => i.PlaylistID == playlist1.PlaylistID).FirstOrDefault();
            string PlaylistName = playlist.PlaylistName;
            Assert.IsTrue(PlaylistName == "Great songs");
        }
    }
}
