﻿using NAudio.Wave;
using System;
using System.Text;

namespace WindesMusic
{
    public class AudioPlayer
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private bool isPlaying = false;
        private float volume = 1;

        public AudioPlayer()
        {
            outputDevice = new WaveOutEvent();
            outputDevice.PlaybackStopped += OnPlaybackStopped;
        }

        public void PlayChosenSong(string songID)
        {
            StringBuilder fileName = new StringBuilder();
            fileName.Append(songID);
            fileName.Append(".mp3");

            if (audioFile != null)
            {
                OnButtonStopClick(this, new EventArgs());
            }

            audioFile = new AudioFileReader(fileName.ToString());
            outputDevice.Init(audioFile);
            outputDevice.Play();
            isPlaying = true;
        }

        //start, and pause and resume button.
        public void OnButtonPlayClick(object sender, EventArgs args)
        {
            if (!isPlaying)
            {
                if (audioFile == null)
                {
                    audioFile = new AudioFileReader("Feint2.mp3");
                    outputDevice.Init(audioFile);
                }
                outputDevice.Play();
                isPlaying = true;
            }
            else
            {
                outputDevice.Pause();
                isPlaying = false;
            }
        }

        //stop button, executes stop function(OnPlayBackStopped).
        public void OnButtonStopClick(object sender, EventArgs args)
        {
            outputDevice?.Stop();
            isPlaying = false;
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
            if (audioFile == null)
                return;
            sliderValue *= audioFile.TotalTime.TotalSeconds;
            int NewTimeSeconds = Convert.ToInt32(Math.Floor(sliderValue));
            int NewTimeMilliSeconds = Convert.ToInt32(Math.Floor((sliderValue - NewTimeSeconds)*1000));
            if (NewTimeSeconds >= audioFile.TotalTime.TotalSeconds)
            {
                OnButtonStopClick(this, new EventArgs());
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
        public double CurrentPlaceInSong()
        {
            if (audioFile == null)
            {
                //OnButtonStopClick doesn't trigger at end of audio file. This ensures the next song can be played.
                OnButtonStopClick(this, new EventArgs());
                return 0;
            }

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





