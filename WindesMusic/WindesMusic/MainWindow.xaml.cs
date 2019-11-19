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

           
        }

        //start, and pause and resume button.
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonPlayClick(sender, e);
        }

        //stop button, executes stop function(OnPlayBackStopped).
        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonStopClick(sender, e);
        }

        //volume slider
        private void Volume_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            audioPlayer.SetVolume((float)e.NewValue / 100);
        }

        private void Place_In_Song_Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var slider = (Slider)sender;
            var change = slider.Value/100;
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

        private void Mute_Button_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Mute();
        }

        private void PlaceInSongSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
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
            Thickness thickness = new Thickness(25, 0, 0, 5);
            Style style = new Style();
            
            for(int i = 0; i < PlaylistIDs.Count; i++)
            {
                var PlaylistButton = new Button
                {
                    //Style = StaticResource MenuButton,
                    Name = $"_{PlaylistIDs[i]}",
                    Content = $"{PlaylistNames[i]}",
                    Margin = thickness
                };
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

            List<string> SongNames = new List<string>();
            SongNames = data.GetRecordsString($"SELECT Name FROM Song WHERE SongID IN(SELECT SongID FROM PlaylistToSong WHERE PlaylistID = {PlaylistID})", "Name");
            Playlist playlist = new Playlist(PlaylistID);
            Thickness thickness = new Thickness(25, 0, 0, 5);
            for(int i = 0; i < playlist.SongPlaylist.Count; i++)
            {
                var SongBlock = new TextBlock
                {
                    Name = $"_{playlist.SongPlaylist[i]}",
                    Text = $"{SongNames[i]}",
                    Margin = thickness
                };
                SongList.Children.Add(SongBlock);
            }
        }

        private void NewPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            NewPlaylistWindow NewPlaylist = new NewPlaylistWindow();
            NewPlaylist.Show();
            NewPlaylist.Closed += (object sender2, EventArgs e2) => OnContentRendered(e);           
        }
    }
}