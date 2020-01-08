using NAudio.Wave;
using System;
using System.Text;

namespace WindesMusic
{
    public class AudioPlayer
    {
        //WaveOutEvent plays audio file read by AudioFileReader.
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private MainWindow mainWindow;
        private bool isPlaying = false;
        private float volume = 1;
        public Song currentSong;

        public AudioPlayer(MainWindow main)
        {
            outputDevice = new WaveOutEvent();
            mainWindow = main;

            //OnPlaybackStopped triggers at the end of an audiofile.
            outputDevice.PlaybackStopped += OnPlaybackStopped;
        }

        //Plays audiofile specifically selected by user.
        public void PlayChosenSong(Song song)
        {
            StringBuilder fileName = new StringBuilder();
            fileName.Append(song.SongID);
            fileName.Append(".mp3");
            currentSong = song;
            DisposeOfSong();
            audioFile = null;

            //tries reading and playing audiofile.
            try
            {
                audioFile = new AudioFileReader(fileName.ToString());
                outputDevice.Init(audioFile);
                outputDevice.Play();
                isPlaying = true;
                mainWindow.Song.Content = song.SongName;
                mainWindow.Artist.Content = song.Artist;
            }
            catch (Exception)
            {
                Console.WriteLine("File not found");
            }
        }

        //Plays next song in MusicQueue.
        public void PlayChosenSong()
        {
            if (MusicQueue.songQueue.Count != 0)
            {
                Song song = MusicQueue.songQueue.Dequeue();
                StringBuilder fileName = new StringBuilder();
                fileName.Append(song.SongID);
                fileName.Append(".mp3");
                currentSong = null;
                DisposeOfSong();
                audioFile = null;

                //tries reading and playing audiofile.
                try
                {
                    audioFile = new AudioFileReader(fileName.ToString());
                    outputDevice.Stop();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    currentSong = song;
                    isPlaying = true;
                    mainWindow.Song.Content = song.SongName;
                    mainWindow.Artist.Content = song.Artist;
                }
                catch (Exception)
                {
                    Console.WriteLine("File not found");
                }
            }
            else if (MusicQueue.recommendedSongQueue.Count != 0)
            {
                Song song = MusicQueue.recommendedSongQueue.Dequeue();
                StringBuilder fileName = new StringBuilder();
                fileName.Append(song.SongID);
                fileName.Append(".mp3");
                currentSong = null;
                DisposeOfSong();
                audioFile = null;
                try
                {
                    audioFile = new AudioFileReader(fileName.ToString());
                    outputDevice.Stop();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    currentSong = song;
                    isPlaying = true;
                    mainWindow.Song.Content = song.SongName;
                    mainWindow.Artist.Content = song.Artist;
                }
                catch (Exception)
                {
                    Console.WriteLine("File not found");
                }
            }
        }

        //pause and resume, and start button.
        public void OnButtonPlayClick(object sender, EventArgs args)
        {
            if (!isPlaying)
            {
                try
                {
                    outputDevice.Play();
                    isPlaying = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("No song to play");
                }
            }
            else
            {
                outputDevice.Pause();
                isPlaying = false;
            }
        }

        //stop button, executes stop function(OnPlayBackStopped).
        public void OnButtonStopClick()
        {
            outputDevice?.Pause();
            outputDevice?.Stop();
            DisposeOfSong();
            audioFile = null;
            isPlaying = false;
            if (currentSong != null)
            {
                MusicQueue.AddSongToPreviousQueue(currentSong);
            }

        }

        //plays next song in MusicQueue.
        public void OnButtonNextClick()
        {
            outputDevice?.Pause();
            outputDevice?.Stop();
            DisposeOfSong();
            audioFile = null;
            isPlaying = false;
            mainWindow.Song.Content = "";
            mainWindow.Artist.Content = "";
            if (currentSong != null)
            {
                MusicQueue.AddSongToPreviousQueue(currentSong);
                if (MusicQueue.isRepeat == true)
                {
                    MusicQueue.songQueue.Clear();
                    MusicQueue.recommendedSongQueue.Clear();
                    MusicQueue.AddSongToQueue(currentSong);
                    this.PlayChosenSong();
                }
            }
            if (MusicQueue.songQueue.Count != 0 && audioFile == null)
            {
                this.PlayChosenSong();

            }
            else if (MusicQueue.recommendedSongQueue.Count != 0 && audioFile == null)
            {
                this.PlayChosenSong();
            }
        }

        //plays last song in MusicQueue.
        public void OnButtonPreviousClick()
        {
            outputDevice?.Stop();
            DisposeOfSong();
            audioFile = null;
            isPlaying = false;

            if (MusicQueue.previousSongs.Count != 0 && audioFile == null)
            {
                this.PlayChosenSong(MusicQueue.previousSongs.Pop());
            }
        }

        //stop function, disposes of AudiofileReader.
        public void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            if (this.CurrentPlaceInSongPercentage() >= 99)
            {
                outputDevice?.Pause();
                outputDevice?.Stop();
                MusicQueue.AddSongToPreviousQueue(currentSong);
                PlayChosenSong();
            }
            if (currentSong == null)
            {

            }
        }

        //recieves change in slider value and calculates new position in song.
        public void SetCurrentPlaceInSong(double sliderValue)
        {
            if (audioFile == null)
                return;
            sliderValue *= audioFile.TotalTime.TotalSeconds;
            int NewTimeSeconds = Convert.ToInt32(Math.Floor(sliderValue));
            int NewTimeMilliSeconds = Convert.ToInt32(Math.Floor((sliderValue - NewTimeSeconds) * 1000));
            if (NewTimeSeconds >= audioFile.TotalTime.TotalSeconds)
            {
                OnButtonStopClick();
                return;
            }
            else if (NewTimeSeconds < 0)
            {
                NewTimeSeconds = 0;
            }
            TimeSpan toPlaceInSong = new TimeSpan(0, 0, 0, NewTimeSeconds, NewTimeMilliSeconds);
            audioFile.CurrentTime = toPlaceInSong;
        }

        //returns percentage of place in song (0-100). 0 means no song is playing. 
        public double CurrentPlaceInSongPercentage()
        {
            if (audioFile == null)
            {
                //OnButtonStopClick doesn't trigger at end of audio file. This ensures the next song can be played.
                OnButtonStopClick();
                return 0;
            }

            return audioFile.CurrentTime.TotalSeconds / audioFile.TotalTime.TotalSeconds * 100;
        }

        public TimeSpan TotalTimeSong() { return audioFile.TotalTime; }

        public TimeSpan CurrentPlaceInSong() { return audioFile.CurrentTime; }

        //volume slider (outputDevice.Volume sends values ranging 0 to 1).
        public void SetVolume(float volume) { outputDevice.Volume = volume; }

        //Saves soundlevel for later and mutes sound.
        public void Mute()
        {
            if (outputDevice.Volume > 0)
            {
                volume = outputDevice.Volume;
                outputDevice.Volume = 0;
            }
            else
            {
                outputDevice.Volume = volume;
            }
        }

        public void DisposeOfSong()
        {
            try
            {
                audioFile.Dispose();
            }
            catch (Exception) { }
        }
    }
}





