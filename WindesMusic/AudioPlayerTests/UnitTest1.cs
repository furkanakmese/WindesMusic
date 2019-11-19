using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Windows.Controls;
using WindesMusic;

namespace AudioPlayerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAudioStartPlayingAndPauses()
        {
            //Play song for 3 seconds.
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(3000);

            //pause song en keep time in song stored.
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            double currentTimeInSong = audioPlayer.CurrentPlaceInSong();
            Thread.Sleep(2000);

            //check if song really stopped playing.
            Assert.AreEqual(currentTimeInSong, audioPlayer.CurrentPlaceInSong(), $"{currentTimeInSong} - {audioPlayer.CurrentPlaceInSong()}");
        }


        [TestMethod]
        public void TestAudioPlaysAfterPause()
        {
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(3000);

            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            double currentTimeInSong = audioPlayer.CurrentPlaceInSong();


            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(1000);
            Assert.IsTrue(currentTimeInSong < audioPlayer.CurrentPlaceInSong());
        }

        [TestMethod]
        public void TestAudioStopsPlayingByButton()
        {
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(3000);

            audioPlayer.OnButtonStopClick(this, new System.EventArgs());
            Thread.Sleep(10);
            Assert.IsTrue(audioPlayer.CurrentPlaceInSong() == 0);
        }

        [TestMethod]
        public void TestAudioStopsPlayingAtEndOfSong()
        {
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(1000);

            audioPlayer.SetCurrentPlaceInSong(1);
            Thread.Sleep(1000);
            Assert.IsTrue(audioPlayer.CurrentPlaceInSong() == 0);
        }
    }
}
