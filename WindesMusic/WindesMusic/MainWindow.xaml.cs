using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
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
using System.Windows.Media.Effects;
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
        public AudioPlayer audioPlayer;
        private DispatcherTimer dispatcherTimer;
        public User user;
        private Database db = new Database();
        private Account account;
        public PlaylistSongsPage playlistSongs = new PlaylistSongsPage();
        private QueuePage queuePage;
        private double sliderVolume;

        public MainWindow()
        {
            InitializeComponent();
            Playlists playlists = new Playlists(this);
            account = new Account();
            sliderVolume = sldVolume.Value;
            audioPlayer = new AudioPlayer(this);
            queuePage = new QueuePage(this);
            //  DispatcherTimer setup
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((object sender, EventArgs e) => {
                PlaceInSongSlider.Value = audioPlayer.CurrentPlaceInSongPercentage();
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();

            Main.Content = playlists;
            account.logout += () => {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            };

            TextInfo info = new CultureInfo("en-US", false).TextInfo;
            inputSearch.KeyDown += (object sender, KeyEventArgs e) => {
                if (e.Key == Key.Enter) Main.Content = new SearchResults(info.ToTitleCase(inputSearch.Text), user, this);
            };
            //btnPlay.Click += (object sender, RoutedEventArgs e) => audioPlayer.OnButtonPlayClick(sender, e);
            //btnMute.Click += (object sender, RoutedEventArgs e) => audioPlayer.Mute();
            sldVolume.ValueChanged += (object sender, RoutedPropertyChangedEventArgs<double> e) => {
                float value = (float)e.NewValue / 100;
                if(value >= .75)
                {
                    PackIconVolume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;
                }else if(value >= .25 && value < .75)
                {
                    PackIconVolume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeMedium;
                }else if(value > 0 && value < .25)
                {
                    PackIconVolume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeLow;
                }else if(value == 0)
                {
                    PackIconVolume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeOff;
                }
                audioPlayer.SetVolume(value);
            };
            //btnAccount.Click += (object sender, RoutedEventArgs e) => Main.Content = account;
            btnPlaylists.Click += (object sender, RoutedEventArgs e) => Main.Content = new Playlists(this);
        }
        
        private void PlaceInSongSliderDragStarted (object sender, DragStartedEventArgs e) => dispatcherTimer.Stop();

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
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonNextClick();
            queuePage.RerenderQueuePage();

        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            PlaylistList.Children.Clear();
            user = db.GetUserData(Properties.Settings.Default.UserID);

            playlistSongs.rerender += (playlist) => { playlistSongs.playlistToUse = playlist; playlistSongs.reinitialize(playlist, this, user); };
            queuePage.rerender += (queuePg) => { queuePage = queuePg; queuePage.InitialiseQueuePage(); };
            Thickness thickness = new Thickness(15, 0, 0, 5);
            foreach (var item in user.Playlists)
            {
                var PlaylistButton = new Button
                {
                    //Style = StaticResource MenuButton,
                    Name = $"_{item.PlaylistID}",
                    Content = $"{item.PlaylistName}",
                    FontSize = 23,
                    Margin = thickness
                };
                StaticResourceExtension menuButton = new StaticResourceExtension("MenuButton");
                PlaylistButton.Style = (Style)FindResource("MenuButton");
                PlaylistButton.Click += ButtonClickPlaylist;
                PlaylistList.Children.Add(PlaylistButton);
            }

            MainGrid.Effect = new BlurEffect{ Radius = 0 };
        }

        public void RerenderPlaylists()
        {
            PlaylistList.Children.Clear();
            user = db.GetUserData(Properties.Settings.Default.UserID);

            playlistSongs.rerender += (playlist) => { playlistSongs.playlistToUse = playlist; playlistSongs.reinitialize(playlist, this, user); };
            queuePage.rerender += (queuePg) => { queuePage = queuePg; queuePage.InitialiseQueuePage(); };
            Thickness thickness = new Thickness(15, 0, 0, 5);
            foreach (var item in user.Playlists)
            {
                var PlaylistButton = new Button
                {
                    //Style = StaticResource MenuButton,
                    Name = $"_{item.PlaylistID}",
                    Content = $"{item.PlaylistName}",
                    FontSize = 23,
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
            Button _ButtonPlaylist = sender as Button;
            int PlaylistId = Convert.ToInt32(_ButtonPlaylist.Name.Substring(1));
            Playlist relevantPlaylist = user.Playlists.Where(i => i.PlaylistID == PlaylistId).FirstOrDefault();
            // playlistSongs = new PlaylistSongsPage();
            playlistSongs.reinitialize(relevantPlaylist, this, user);
            Main.Content = playlistSongs;
        }

        private void ButtonClickQueue(object sender, RoutedEventArgs e)
        {
            if (Main.Content != queuePage)
            {
                queuePage.InitialiseQueuePage();
                Main.Content = queuePage;
            }
            else
            {
                if (playlistSongs != null)
                {
                    Main.Content = playlistSongs;
                }
                else
                {
                    Playlists playlists = new Playlists(this);
                    Main.Content = playlists;
                }
            }
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.OnButtonPlayClick(sender, e);
            if(PackIconPlay.Kind == MaterialDesignThemes.Wpf.PackIconKind.Play)
            {
                PackIconPlay.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            else
            {
                PackIconPlay.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            }
            
        }

        private void ShuffleButtonClick(object sender, RoutedEventArgs e)
        {
            if (MusicQueue.IsShuffle == false)
            {
                btnShuffle.Background = new SolidColorBrush(System.Windows.Media.Colors.DarkOrange);
                PackIconShuffle.Kind = MaterialDesignThemes.Wpf.PackIconKind.ShuffleVariant;
                MusicQueue.ShuffleSongs();
                MusicQueue.IsShuffle = true;
            }
            else
            {
                btnShuffle.Background = new SolidColorBrush(System.Windows.Media.Colors.DimGray);
                PackIconShuffle.Kind = MaterialDesignThemes.Wpf.PackIconKind.ShuffleDisabled;
                MusicQueue.IsShuffle = false;
            }
        }

        private void MuteButtonClick(object sender, RoutedEventArgs e)
        {
            audioPlayer.Mute();

            if (PackIconVolume.Kind == MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh ||
               PackIconVolume.Kind == MaterialDesignThemes.Wpf.PackIconKind.VolumeMedium ||
               PackIconVolume.Kind == MaterialDesignThemes.Wpf.PackIconKind.VolumeLow)
            {
                PackIconVolume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeOff;
                sliderVolume = sldVolume.Value;
                sldVolume.Value = 0;
            }
            else
            {
                PackIconVolume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;
                sldVolume.Value = sliderVolume;
            }
        }
        private void RepeatButtonClick(object sender, RoutedEventArgs e)
        {
            if (MusicQueue.IsRepeat == false)
            {
                btnRepeat.Background = new SolidColorBrush(System.Windows.Media.Colors.DarkOrange);
                MusicQueue.IsRepeat = true;
            }
            else
            {
                btnRepeat.Background = new SolidColorBrush(System.Windows.Media.Colors.LightGray);
                MusicQueue.IsRepeat = false;
            }
        }

        private void NewPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            NewPlaylistWindow NewPlaylist = new NewPlaylistWindow();
            NewPlaylist.Show();
            
            MainGrid.Effect = new BlurEffect { Radius = 10 };
            NewPlaylist.Closed += (object sender2, EventArgs e2) => OnContentRendered(e); 
        }
        private void HistoryPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            Main.Content = new DailyHistoryPlaylistPage(user, this);
        }

        private void ButtonClickAccount(object sender, RoutedEventArgs e)
        {
            Main.Content = account;
        }

        private void DailyPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            Main.Content = new DailyPlaylistPage(user, this);
        }

        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            DropShadowEffect shadow = new DropShadowEffect { BlurRadius = 2, Opacity = .5 };
            Console.WriteLine(e.Source.GetHashCode());
            
            
        }
    }
}