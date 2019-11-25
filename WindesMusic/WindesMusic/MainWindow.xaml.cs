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
        public AudioPlayer audioPlayer = new AudioPlayer();
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
            inputSearch.KeyDown += InputSearch_KeyDown;
        }

        private void InputSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Main.Content = new SearchResults(inputSearch.Text);
            }
        }

        //start, and pause and resume button.
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonPlayClick(sender, e);
        }

        //stop button, executes stop function(OnPlayBackStopped).
        private void StopButtonClick()
        {
            audioPlayer.OnButtonStopClick();
        }

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            StopButtonClick();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            StopButtonClick();
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
            PlaceInSongSlider.Value = audioPlayer.CurrentPlaceInSongPercentage();
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
            User user = database1.GetUserData(WindesMusic.Properties.Settings.Default.UserID);
            Thickness thickness = new Thickness(15, 0, 0, 5);
            foreach (var item in user.Playlists)
            {
                var PlaylistButton = new Button
                {
                    //Style = StaticResource MenuButton,
                    Name = $"_{item.PlaylistID}",
                    Content = $"{item.PlaylistName}",
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
            List<Song> ListOfSongs = data.GetSongsInPlaylist(PlaylistID);
            string PlaylistName = data.GetNameFromPlaylist(PlaylistID);
            PlaylistSongsPage SongsPage = new PlaylistSongsPage(PlaylistID, PlaylistName, ListOfSongs, this);
            this.Main.Content = SongsPage;
            
            
        }

        private void NewPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            NewPlaylistWindow NewPlaylist = new NewPlaylistWindow();
            NewPlaylist.Show();
            NewPlaylist.Closed += (object sender2, EventArgs e2) => OnContentRendered(e);
        }
    }
}