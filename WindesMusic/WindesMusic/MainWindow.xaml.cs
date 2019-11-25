using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AudioPlayer audioPlayer = new AudioPlayer();
        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            //  DispatcherTimer setup
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateSongSlider);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();

            Main.Content = new Playlists();
        }

        //start, and pause and resume button.
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonPlayClick(sender, e);
        }

        //stop button, executes stop function(OnPlayBackStopped).
        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonStopClick(sender, e);
        }

        //volume slider
        private void VolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            audioPlayer.SetVolume((float)e.NewValue / 100);
        }

        private void PlaceInSongSliderDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var slider = (Slider)sender;
            var change = slider.Value / 100;
            if (change >= PlaceInSongSlider.Maximum)
            {
                audioPlayer.SetCurrentPlaceInSong(PlaceInSongSlider.Maximum);
            }
            else
            {
                audioPlayer.SetCurrentPlaceInSong(change);
            }
            dispatcherTimer.Start();
        }

        private void UpdateSongSlider(object sender, EventArgs e)
        {
            PlaceInSongSlider.Value = audioPlayer.CurrentPlaceInSong();
        }

        private void MuteButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.Mute();
        }

        private void PlaceInSongSliderDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dispatcherTimer.Stop();
        }
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Database database1 = new Database();
            List<int> PlaylistIDs = new List<int>();
            List<string> PlaylistNames = new List<string>();
            PlaylistList.Children.Clear();
            PlaylistIDs = database1.GetRecordsInt($"SELECT PlaylistID FROM Playlist WHERE UserID = 1", "PlaylistID");
            PlaylistNames = database1.GetRecordsString($"SELECT PlaylistName FROM Playlist WHERE UserID = 1", "PlaylistName");
            Thickness thickness = new Thickness(15, 0, 0, 5);

            for (int i = 0; i < PlaylistIDs.Count; i++)
            {
                var PlaylistButton = new Button
                {
                    //Style = StaticResource MenuButton,
                    Name = $"_{PlaylistIDs[i]}",
                    Content = $"{PlaylistNames[i]}",
                    Margin = thickness
                   
                };
                StaticResourceExtension menuButton = new StaticResourceExtension("MenuButton");
                PlaylistButton.Style = (Style)FindResource("MenuButton");
                PlaylistButton.Click += ButtonClickPlaylist;
                PlaylistList.Children.Add(PlaylistButton);
            }
        }

        private void ButtonClickPlaylist(object sender, RoutedEventArgs e)
        {
            SongList.Children.Clear();
            Database data = new Database();
            Button _ButtonPlaylist = sender as Button;
            string PlaylistIDName = _ButtonPlaylist.Name;
            PlaylistIDName = PlaylistIDName.Substring(1);
            int PlaylistID = Convert.ToInt32(PlaylistIDName);

            Playlist playlist = new Playlist(PlaylistID);
            playlist.SongPlaylist = data.GetSongsInPlaylist(PlaylistID);
            Thickness thickness = new Thickness(10, 2, 0, 5);
            Thickness thickness2 = new Thickness(10, 0, 0, 5);
            for (int i = 0; i < playlist.SongPlaylist.Count; i++)
            {
                Song playlistSong = playlist.SongPlaylist[i];
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.VerticalAlignment = VerticalAlignment.Stretch;
                sp.HorizontalAlignment = HorizontalAlignment.Stretch;
                var PlayButton = new Button
                {
                    Name = $"__{playlistSong.SongID}",
                    Content = "Play",
                    Margin = thickness2
                };
                PlayButton.Click += PlaySongFromPlaylist;
                var SongBlockName = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.SongName}",
                    Margin = thickness
                };
                var SongBlockArtist = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Artist}",
                    Margin = thickness
                };
                var SongBlockAlbum = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Album}",
                    Margin = thickness
                };
                var SongBlockYear = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Year}",
                    Margin = thickness
                };
                sp.Children.Add(PlayButton);
                sp.Children.Add(SongBlockName);
                sp.Children.Add(SongBlockArtist);
                sp.Children.Add(SongBlockAlbum);
                sp.Children.Add(SongBlockYear);
                SongList.Children.Add(sp);
            }
        }

        private void NewPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            NewPlaylistWindow NewPlaylist = new NewPlaylistWindow();
            NewPlaylist.Show();
            NewPlaylist.Closed += (object sender2, EventArgs e2) => OnContentRendered(e);
        }

        private void PlaySongFromPlaylist(object sender, RoutedEventArgs e)
        {
            Button _ButtonSong = sender as Button;
            string SongID = _ButtonSong.Name;
            SongID = SongID.Substring(2);
            //int Song = Convert.ToInt32(SongID);

            audioPlayer.PlayChosenSong(SongID);
        }
    }
}