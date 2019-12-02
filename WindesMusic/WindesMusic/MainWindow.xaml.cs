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
        private User user;
        private Database db = new Database();
        private Account account = new Account();

        public MainWindow()
        {
            InitializeComponent();

            //  DispatcherTimer setup
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((object sender, EventArgs e) => {
                PlaceInSongSlider.Value = audioPlayer.CurrentPlaceInSongPercentage();
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();

            Playlists playlists = new Playlists();
            Main.Content = playlists;
            account.logout += () => {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            };

            inputSearch.KeyDown += (object sender, KeyEventArgs e) => {
                if (e.Key == Key.Enter) Main.Content = new SearchResults(inputSearch.Text);
            };
            btnPlay.Click += (object sender, RoutedEventArgs e) => audioPlayer.OnButtonPlayClick(sender, e);
            btnMute.Click += (object sender, RoutedEventArgs e) => audioPlayer.Mute();
            sldVolume.ValueChanged += (object sender, RoutedPropertyChangedEventArgs<double> e) => {
                audioPlayer.SetVolume((float)e.NewValue / 100);
            };
            btnAccount.Click += (object sender, RoutedEventArgs e) => Main.Content = account;
            btnPlaylists.Click += (object sender, RoutedEventArgs e) => Main.Content = new Playlists();
        }

        private void PlaceInSongSliderDragStarted(object sender, DragStartedEventArgs e) => dispatcherTimer.Stop();

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

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonPreviousClick();

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

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            PlaylistList.Children.Clear();
            user = db.GetUserData(Properties.Settings.Default.UserID);

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
            Button _ButtonPlaylist = sender as Button;
            int PlaylistId = Convert.ToInt32(_ButtonPlaylist.Name.Substring(1));
            Playlist relevantPlaylist = user.Playlists.Where(i => i.PlaylistID == PlaylistId).FirstOrDefault();
            PlaylistSongsPage SongsPage = new PlaylistSongsPage(relevantPlaylist, this, user);
            this.Main.Content = SongsPage;
        }

        private void NewPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            NewPlaylistWindow NewPlaylist = new NewPlaylistWindow();
            NewPlaylist.Show();
            NewPlaylist.Closed += (object sender2, EventArgs e2) => OnContentRendered(e);
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Account();
        }
    }
}