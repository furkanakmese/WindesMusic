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
    class QueueTests
    {
        [TestMethod]
        public void TestAddSongToQueue()
        {
            //Arrange
            //Create int to see how many songs were in the queue
            Database db = new Database();
            int AmountOfSongs = MusicQueue.SongQueue.Count();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song = songs[1];

            //Act
            //Add song with id 1 to the queue
            MusicQueue.AddSongToQueue(song);

            //Assert
            //Check if amount of songs is 1 more than earlier
            int NewAmountOfSongs = MusicQueue.SongQueue.Count();
            Assert.IsTrue(NewAmountOfSongs == (AmountOfSongs + 1));
        }

        [TestMethod]
        public void TestAddCorrectSongToQueue()
        {
            //Arrange
            //Empty queue and add song with id 5 to the queue
            Database db = new Database();
            MusicQueue.SongQueue.Clear();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song = songs[1];
            MusicQueue.AddSongToQueue(song);

            //Act
            //Get Song from Queue
            Song song1 = MusicQueue.SongQueue.Peek();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(song.SongID == song1.SongID);
        }

        [TestMethod]
        public void TestGetSongFromQueue()
        {
            //Arrange
            //Empty queue and add songs with id 5, 3 and 2 to the queue
            Database db = new Database();
            MusicQueue.SongQueue.Clear();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song1 = songs[1];
            Song song2 = songs[2];
            Song song3 = songs[3];
            MusicQueue.AddSongToQueue(song1);
            MusicQueue.AddSongToQueue(song2);
            MusicQueue.AddSongToQueue(song3);
            int AmountOfSongs = MusicQueue.SongQueue.Count();

            //Act
            //Remove song from queue
            MusicQueue.GetSongFromQueue();

            //Assert
            //Check if the queue has become 1 shorter
            int NewAmountOfSongs = MusicQueue.SongQueue.Count();
            Assert.IsTrue(NewAmountOfSongs == (AmountOfSongs - 1));
        }

        [TestMethod]
        public void TestGetCorrectSongFromQueue()
        {
            //Arrange
            //Empty queue and add song to the queue
            Database db = new Database();
            MusicQueue.SongQueue.Clear();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song1 = songs[1];
            MusicQueue.AddSongToQueue(song1);

            //Act
            //Get Song from Queue
            Song song = MusicQueue.GetSongFromQueue();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(song1.SongID == song.SongID);
        }

        [TestMethod]
        public void TestAddSongToPreviousQueue()
        {
            //Arrange
            //Create int to see how many songs were in the queue
            Database db = new Database();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song = songs[1];
            int AmountOfSongs = MusicQueue.PreviousSongs.Count();

            //Act
            //Add song with id 1 to the queue
            MusicQueue.AddSongToPreviousQueue(song);

            //Assert
            //Check if amount of songs is 1 more than earlier
            int NewAmountOfSongs = MusicQueue.PreviousSongs.Count();
            Assert.IsTrue(NewAmountOfSongs == (AmountOfSongs + 1));
        }

        [TestMethod]
        public void TestAddCorrectSongToPreviousQueue()
        {
            //Arrange
            //Empty queue and add song with id 5 to the queue
            Database db = new Database();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song = songs[1];
            MusicQueue.PreviousSongs.Clear();
            MusicQueue.AddSongToPreviousQueue(song);

            //Act
            //Get Song from Queue
            Song Song1 = MusicQueue.PreviousSongs.Peek();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(song.SongID == Song1.SongID);
        }

        [TestMethod]
        public void TestGetSongFromPreviousQueue()
        {
            //Arrange
            //Empty queue and add songs with id 5, 3 and 2 to the queue
            Database db = new Database();
            MusicQueue.PreviousSongs.Clear();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song1 = songs[1];
            Song song2 = songs[2];
            Song song3 = songs[3];
            MusicQueue.AddSongToPreviousQueue(song1);
            MusicQueue.AddSongToPreviousQueue(song2);
            MusicQueue.AddSongToPreviousQueue(song3);
            int AmountOfSongs = MusicQueue.PreviousSongs.Count();

            //Act
            //Remove song from queue
            MusicQueue.PreviousSongs.Pop();

            //Assert
            //Check if the queue has become 1 shorter
            int NewAmountOfSongs = MusicQueue.PreviousSongs.Count();
            Assert.IsTrue(NewAmountOfSongs == (AmountOfSongs - 1));
        }

        [TestMethod]
        public void TestGetCorrectSongFromPreviousQueue()
        {
            //Arrange
            //Empty queue and add song with id 3 to the queue
            Database db = new Database();
            List<Song> songs = db.GetSongsInPlaylist(1);
            Song song1 = songs[1];
            MusicQueue.PreviousSongs.Clear();
            MusicQueue.AddSongToPreviousQueue(song1);

            //Act
            //Get Song from Queue
            Song song = MusicQueue.PreviousSongs.Pop();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(song1.SongID == song.SongID);
        }
    }
}
