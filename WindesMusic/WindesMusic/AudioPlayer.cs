using NAudio.Wave;
using System;

namespace WindesMusic
{
    class AudioPlayer
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private bool isPlaying = true;
        private float volume = 1;

        public AudioPlayer()
        {
            outputDevice = new WaveOutEvent();
            outputDevice.PlaybackStopped += OnPlaybackStopped;
        }

        //start, and pause and resume button.
        public void OnButtonPlayClick(object sender, EventArgs args)
        {
            if (isPlaying)
            {
                if (audioFile == null)
                {
                    audioFile = new AudioFileReader("Feint - Words.mp3");
                    outputDevice.Init(audioFile);
                }
                outputDevice.Play();
                isPlaying = false;
            }
            else
            {
                outputDevice.Pause();
                isPlaying = true;
            }
        }

        //stop button, executes stop function(OnPlayBackStopped).
        public void OnButtonStopClick(object sender, EventArgs args)
        {
            outputDevice?.Stop();
            isPlaying = true;
        }

        //stop function, disposes of AudiofileReader.
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            audioFile.Dispose();
            audioFile = null;
        }

        //recieves change in slider value and calculates new position in song.
        public void SetCurrentPlaceInSong(double sliderValue)
        {
            sliderValue = sliderValue / 100 * audioFile.TotalTime.TotalSeconds;
            double currentPlaceInSong = audioFile.CurrentTime.TotalSeconds;
            int newTime;
            if (sliderValue + currentPlaceInSong > audioFile.TotalTime.TotalSeconds)
            {
                newTime = (int)audioFile.TotalTime.TotalSeconds;
            }
            else if (sliderValue + currentPlaceInSong < 0)
            {
                newTime = 0;
            }
            else
            {
                newTime = (int)(sliderValue + currentPlaceInSong);
            }
            TimeSpan toPlaceInSong = new TimeSpan(0, 0, 0, newTime, 0);
            audioFile.CurrentTime = toPlaceInSong;
        }

        //returns percentage of place in song.
        public double CurrentPlaceInSong()
        {
            if (audioFile == null)
                return 0;
            return audioFile.CurrentTime.TotalSeconds / audioFile.TotalTime.TotalSeconds * 100;
        }

        //volume slider (outputDevice.Volume has values 0 - 1).
        public void SetVolume(float volume)
        {
            outputDevice.Volume = volume;
        }

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
    }
}





