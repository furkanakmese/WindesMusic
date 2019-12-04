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
            int AmountOfSongs = MusicQueue.SongQueue.Count();

            //Act
            //Add song with id 1 to the queue
            MusicQueue.AddSongToQueue(1);

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
            MusicQueue.SongQueue.Clear();
            MusicQueue.AddSongToQueue(5);

            //Act
            //Get Song from Queue
            int SongID = MusicQueue.SongQueue.Peek();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(SongID == 5);
        }

        [TestMethod]
        public void TestGetSongFromQueue()
        {
            //Arrange
            //Empty queue and add songs with id 5, 3 and 2 to the queue
            MusicQueue.SongQueue.Clear();
            MusicQueue.AddSongToQueue(5);
            MusicQueue.AddSongToQueue(3);
            MusicQueue.AddSongToQueue(2);
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
            //Empty queue and add song with id 3 to the queue
            MusicQueue.SongQueue.Clear();
            MusicQueue.AddSongToQueue(3);

            //Act
            //Get Song from Queue
            int SongID = MusicQueue.GetSongFromQueue();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(SongID == 3);
        }

        [TestMethod]
        public void TestAddSongToPreviousQueue()
        {
            //Arrange
            //Create int to see how many songs were in the queue
            int AmountOfSongs = MusicQueue.PreviousSongs.Count();

            //Act
            //Add song with id 1 to the queue
            MusicQueue.AddSongToPreviousQueue(1);

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
            MusicQueue.PreviousSongs.Clear();
            MusicQueue.AddSongToPreviousQueue(5);

            //Act
            //Get Song from Queue
            int SongID = MusicQueue.PreviousSongs.Peek();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(SongID == 5);
        }

        [TestMethod]
        public void TestGetSongFromPreviousQueue()
        {
            //Arrange
            //Empty queue and add songs with id 5, 3 and 2 to the queue
            MusicQueue.PreviousSongs.Clear();
            MusicQueue.AddSongToPreviousQueue(5);
            MusicQueue.AddSongToPreviousQueue(3);
            MusicQueue.AddSongToPreviousQueue(2);
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
            MusicQueue.PreviousSongs.Clear();
            MusicQueue.AddSongToPreviousQueue(3);

            //Act
            //Get Song from Queue
            int SongID = MusicQueue.PreviousSongs.Pop();

            //Assert
            //Check if the SongIDs match
            Assert.IsTrue(SongID == 3);
        }
    }
}
