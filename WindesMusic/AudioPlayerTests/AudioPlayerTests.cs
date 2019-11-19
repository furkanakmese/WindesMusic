using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using WindesMusic;

namespace UnitTestWindesMusic
{
    [TestClass]
    public class AudioPlayerTests
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

            //check if song stopped playing.
            Assert.AreEqual(currentTimeInSong, audioPlayer.CurrentPlaceInSong(), $"{currentTimeInSong} - {audioPlayer.CurrentPlaceInSong()}");
        }


        [TestMethod]
        public void TestAudioPlaysAfterPause()
        {
            //play song for 3 seconds.
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(3000);

            //pause playing song.
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            double currentTimeInSong = audioPlayer.CurrentPlaceInSong();

            //resume playing song.
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(1000);
            Assert.IsTrue(currentTimeInSong < audioPlayer.CurrentPlaceInSong());
        }

        [TestMethod]
        public void TestAudioStopsPlayingByButton()
        {
            //play song for 3 seconds.
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(3000);

            //press stop button and check if song stopped playing.
            audioPlayer.OnButtonStopClick(this, new System.EventArgs());
            Thread.Sleep(10);
            Assert.IsTrue(audioPlayer.CurrentPlaceInSong() == 0);
        }

        [TestMethod]
        public void TestAudioStopsPlayingAtEndOfSong()
        {
            //play song for 1 second.
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(1000);

            //simulate slider set song to end and check if song stopped playing.
            audioPlayer.SetCurrentPlaceInSong(1);
            Thread.Sleep(1000);
            Assert.IsTrue(audioPlayer.CurrentPlaceInSong() == 0);
        }

        [TestMethod]
        public void TestSliderValueToRightPlaceInSong()
        {
            //play song.
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());

            //pause song so an accurate place in song can be measured.
            audioPlayer.OnButtonPlayClick(this, new System.EventArgs());
            Thread.Sleep(1000);

            //simulate slider set song to halfway through.
            audioPlayer.SetCurrentPlaceInSong(.5);
            Thread.Sleep(1000);

            //a millisecond inaccuracy allowed. 
            Assert.AreEqual(50, audioPlayer.CurrentPlaceInSong(), 0.001);
        }
    }
}
