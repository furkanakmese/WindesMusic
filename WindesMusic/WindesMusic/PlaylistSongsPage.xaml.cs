using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class PlaylistSongsPage : Page
    {
        Database db = new Database();
        public Playlist playlistToUse;
        private string _orderBy;
        private int _PlaylistID;
        private string _PlaylistName;
        public List<Song> SongsInPlaylist = new List<Song>();
        List<Song> RecommendedSongs = new List<Song>();
        List<Song> RecommendedAds = new List<Song>();
        MainWindow mainWindow;
        string orderBy;
        User user;

        public delegate void OnRerender(Playlist playlist);
        public event OnRerender rerender;

        public PlaylistSongsPage()
        {
            InitializeComponent();
        }

        public void reinitialize(Playlist playlist, MainWindow main, User BaseUser)
        {
            InitializeComponent();
            SongList.Children.Clear();
            PlaylistName.Children.Clear();
            DeletePlaylist.Children.Clear();
            RecommendedSongList.Children.Clear();

            Recommender recommender = new Recommender(db);
            RecommendedSongs = recommender.GetRecommendedSongsForPlaylist(playlist, 5);
            RecommendedAds = recommender.GetRecommendedAdsFromPlaylist(playlist);
            playlistToUse = playlist;
            mainWindow = main;
            user = BaseUser;
            _PlaylistName = playlistToUse.PlaylistName;
            _PlaylistID = playlistToUse.PlaylistID;
            switch (orderBy)
            {
                case "name":
                    playlistToUse.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.SongName).ToList();
                    break;
                case "album":
                    playlistToUse.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.Album).ToList();
                    break;
                case "year":
                    playlistToUse.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.Year).ToList();
                    break;
                case "artist":
                    playlistToUse.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.Artist).ToList();
                    break;
                default:
                    playlistToUse.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.SongID).ToList();
                    break;
            }
            user = BaseUser;
            SongsInPlaylist = playlist.SongPlaylist;
            Thickness SongBlockThickness = new Thickness(5, 2, 0, 0);
            SolidColorBrush whiteText = new SolidColorBrush(System.Windows.Media.Colors.White);
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            StackPanel sp1 = new StackPanel();
            sp1.Orientation = Orientation.Horizontal;
            var PlaylistBlock = new TextBlock
            {
                Text = $"{playlistToUse.PlaylistName}",
                FontSize = 25,
                Foreground = whiteText,
                Margin = new Thickness(0, 10, 0, 5)
            };
            var PlayPlaylistButton = new Button
            {
                Name = $"_{_PlaylistID}",
                Content = "Play",
                FontSize = 30,
                Margin = new Thickness(0, 10, 25, 0),
                Padding = new Thickness(5),
                BorderThickness = new Thickness(0),
                Height = 50,
                Width = 100
            };
            PlayPlaylistButton.Click += PlayPlaylist;
            var DeletePlaylistButton = new Button
            {
                Name = $"Delete_{_PlaylistID}",
                Content = "Delete",
                Tag = playlistToUse,
                FontSize = 15,
                Margin = new Thickness(10, 10, 0, 5)
            };
            DeletePlaylistButton.Click += DeletePlaylistClick;

            var RenamePlaylistButton = new Button
            {
                Name = $"Rename_{_PlaylistID}",
                Content = "Rename",
                Tag = playlistToUse,
                FontSize = 15,
                Margin = new Thickness(-50, 10, 0, 5)
            };
            RenamePlaylistButton.Click += RenamePlaylist;

            sp.Children.Add(PlaylistBlock);
            sp.Children.Add(PlayPlaylistButton);
            sp1.Children.Add(RenamePlaylistButton);
            sp1.Children.Add(DeletePlaylistButton);
            DeletePlaylist.Children.Add(sp1);
            PlaylistName.Children.Add(sp);

            OrderList.RowDefinitions.Add(new RowDefinition());

            // Add the Songname text block to the Songlist grid
            var OrderName = new TextBlock
            {
                Name = "Name",
                Text = "Name",
                Foreground = whiteText,
                Margin = SongBlockThickness,
                FontSize = 15
            };
            Grid.SetRow(OrderName, 0);
            Grid.SetColumn(OrderName, 1);
            OrderName.MouseLeftButtonUp += (sender, args) => { _orderBy = "name"; OnLabelClick(sender, args); };

            // Add the artist text block to the Songlist grid
            var OrderArtist = new TextBlock
            {
                Name = $"Artist",
                Text = $"Artist",
                Foreground = whiteText,
                Margin = SongBlockThickness,
                FontSize = 15
            };
            Grid.SetRow(OrderArtist, 0);
            Grid.SetColumn(OrderArtist, 2);
            OrderArtist.MouseLeftButtonUp += (sender, args) => { _orderBy = "artist"; OnLabelClick(sender, args); };

            // Add the album text block to the Songlist grid
            var OrderAlbum = new TextBlock
            {
                Name = $"Album",
                Text = $"Album",
                Foreground = whiteText,
                Margin = SongBlockThickness,
                FontSize = 15
            };
            Grid.SetRow(OrderAlbum, 0);
            Grid.SetColumn(OrderAlbum, 3);
            OrderAlbum.MouseLeftButtonUp += (sender, args) => { _orderBy = "album"; OnLabelClick(sender, args); };

            // Add the year text block to the Songlist grid
            var OrderYear = new TextBlock
            {
                Name = $"Year",
                Text = $"Year",
                Foreground = whiteText,
                Margin = SongBlockThickness,
                FontSize = 15
            };
            Grid.SetRow(OrderYear, 0);
            Grid.SetColumn(OrderYear, 4);
            OrderYear.MouseLeftButtonUp += (sender, args) => { _orderBy = "year"; OnLabelClick(sender, args); };

            // Add the elements to the Songlist grid Children collection
            OrderList.Children.Add(OrderName);
            OrderList.Children.Add(OrderArtist);
            OrderList.Children.Add(OrderAlbum);
            OrderList.Children.Add(OrderYear);

            //Adds the necessary amount of rows for the playlist
            for (int i = 0; i < SongsInPlaylist.Count; i++)
            {
                Song playlistSong = SongsInPlaylist[i];
                RowDefinition rowDef = new RowDefinition();
                rowDef.Name = $"Row_{i}";
                SongList.RowDefinitions.Add(rowDef);
                RowDefinitionCollection RowNames = SongList.RowDefinitions;
                Array RowArray = RowNames.ToArray();

                SolidColorBrush btnTextColor = new SolidColorBrush();
                btnTextColor.Color = Color.FromRgb(0,0,0);

                // Add the play button to the Songlist grid
                var PlayButton = new Button
                {
                    Name = $"__{playlistSong.SongID}",
                    Content = "Play",
                    Margin = new Thickness(5, 0, 0, 5),
                    Padding = new Thickness(5),
                    BorderThickness = new Thickness(0),
                    FontSize = 15,
                    
                    Tag = playlistSong
                };
                Grid.SetRow(PlayButton, i);
                Grid.SetColumn(PlayButton, 0);
                PlayButton.Click += PlaySongFromPlaylist;

                // Add the Songname text block to the Songlist grid
                var SongBlockName = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.SongName}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockName, i);
                Grid.SetColumn(SongBlockName, 1);

                // Add the artist text block to the Songlist grid
                var SongBlockArtist = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Artist}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockArtist, i);
                Grid.SetColumn(SongBlockArtist, 2);

                // Add the album text block to the Songlist grid
                var SongBlockAlbum = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Album}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockAlbum, i);
                Grid.SetColumn(SongBlockAlbum, 3);

                // Add the year text block to the Songlist grid
                var SongBlockYear = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Year}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockYear, i);
                Grid.SetColumn(SongBlockYear, 4);

                // Add the elements to the Songlist grid Children collection
                SongList.Children.Add(PlayButton);
                SongList.Children.Add(SongBlockName);
                SongList.Children.Add(SongBlockArtist);
                SongList.Children.Add(SongBlockAlbum);
                SongList.Children.Add(SongBlockYear);

                SongList.MouseRightButtonDown -= SongContextMenuOpening;
                SongList.MouseRightButtonDown += new MouseButtonEventHandler(SongContextMenuOpening);
            }

            List<Song> RecommendedSongsAndAds = new List<Song>(RecommendedAds);
            foreach(Song song in RecommendedSongs)
            {
                RecommendedSongsAndAds.Add(song);
            }

            for (int i = 0; i < RecommendedSongsAndAds.Count(); i++)
            {
                int amount = RecommendedAds.Count;
                Song playlistSong = RecommendedSongsAndAds[i];
                RowDefinition rowDef = new RowDefinition();
                rowDef.Name = $"Row_{i}";
                RecommendedSongList.RowDefinitions.Add(rowDef);
                RowDefinitionCollection RowNames = RecommendedSongList.RowDefinitions;
                Array RowArray = RowNames.ToArray();

                // Add the play button to the Songlist grid
                var PlayButton = new Button
                {
                    Name = $"__{playlistSong.SongID}",
                    Content = "Play",
                    Margin = new Thickness(5, 0, 0, 5),
                    Padding = new Thickness(5),
                    BorderThickness = new Thickness(0),
                    FontSize = 15,
                    Tag = playlistSong
                };
                Grid.SetRow(PlayButton, i);
                Grid.SetColumn(PlayButton, 0);
                PlayButton.Click += (ob, s) => { db.AddCreditsFromSongClick(true, playlistSong.SongID); PlaySongFromPlaylist(ob, s); };

                var SongBlockName = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                // Add the Songname text block to the Songlist grid
                if (i < amount)
                {
                    SongBlockName.Text = $"(Ad) {playlistSong.SongName}";
                    db.UpdateTimesDisplayedAd(playlistSong.SongID);
                }
                else
                {
                    SongBlockName.Text = $"{playlistSong.SongName}";
                }
                Grid.SetRow(SongBlockName, i);
                Grid.SetColumn(SongBlockName, 1);

                // Add the artist text block to the Songlist grid
                var SongBlockArtist = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Artist}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockArtist, i);
                Grid.SetColumn(SongBlockArtist, 2);

                // Add the album text block to the Songlist grid
                var SongBlockAlbum = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Album}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockAlbum, i);
                Grid.SetColumn(SongBlockAlbum, 3);

                // Add the year text block to the Songlist grid
                var SongBlockYear = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Year}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockYear, i);
                Grid.SetColumn(SongBlockYear, 4);

                // Add the elements to the Songlist grid Children collection
                RecommendedSongList.Children.Add(PlayButton);
                RecommendedSongList.Children.Add(SongBlockName);
                RecommendedSongList.Children.Add(SongBlockArtist);
                RecommendedSongList.Children.Add(SongBlockAlbum);
                RecommendedSongList.Children.Add(SongBlockYear);

                ContextMenu menu = new ContextMenu();
                menu.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
                menu.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);

                RecommendedSongList.ContextMenu = null;
                RecommendedSongList.MouseRightButtonDown += new MouseButtonEventHandler(SongContextMenuFromRecommended);
            }
            
        }
        private void OnLabelClick(object sender, EventArgs args)
        {
            orderBy = _orderBy;
            if (rerender != null)
            {
                rerender(playlistToUse);
            }
        }

        private void SongContextMenuOpening(object sender, MouseButtonEventArgs e)
        {
            List<Playlist> Playlists = user.Playlists;
            var pos = e.GetPosition(SongList);
            double top = pos.Y;
            int top1 = (int)Math.Round(top);
            int amount = top1 / 28;
            ContextMenu menu = new ContextMenu();
            menu.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
            menu.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
            Song song = SongsInPlaylist.ElementAt(amount);
            int CorrectSongID = SongsInPlaylist.ElementAt(amount).SongID;

            MenuItem PlaylistItem = new MenuItem();
            PlaylistItem.Name = $"Playlists";
            PlaylistItem.Header = "Add to Playlist";
            PlaylistItem.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
            PlaylistItem.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);

            foreach (Playlist pl in Playlists)
            {
                MenuItem OnePlaylistItem = new MenuItem();
                OnePlaylistItem.Name = $"Playlist_{pl.PlaylistID}";
                OnePlaylistItem.Tag = song;
                OnePlaylistItem.Header = $"{pl.PlaylistName}";
                OnePlaylistItem.Click += AddToPlaylistClick;
                OnePlaylistItem.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
                OnePlaylistItem.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                PlaylistItem.Items.Add(OnePlaylistItem);
            }

            MenuItem QueueItem = new MenuItem();
            QueueItem.Name = $"Queue_{CorrectSongID}";
            QueueItem.Header = "Add to Queue";
            QueueItem.Tag = song;
            QueueItem.Click += AddToQueueClick;

            MenuItem DeleteItem = new MenuItem();
            DeleteItem.Name = $"Delete_{playlistToUse.PlaylistID}";
            DeleteItem.Header = "Delete from playlist";
            DeleteItem.Tag = song;
            DeleteItem.Click += DeleteFromPlaylistClick;

            menu.Items.Add(PlaylistItem);
            menu.Items.Add(QueueItem);
            menu.Items.Add(DeleteItem);
            SongList.ContextMenu = menu;
        }

        private void SongContextMenuFromRecommended(object sender, MouseButtonEventArgs e)
        {
            List<Playlist> Playlists = user.Playlists;
            var pos = e.GetPosition(RecommendedSongList);
            double top = pos.Y;
            int top1 = (int)Math.Round(top);
            int amount = top1 / 28;
            ContextMenu menu = new ContextMenu();
            Song song = RecommendedSongs.ElementAt(amount);
            int CorrectSongID = RecommendedSongs.ElementAt(amount).SongID;

            MenuItem PlaylistItem = new MenuItem();
            PlaylistItem.Name = $"Playlists";
            PlaylistItem.Header = "Add to Playlist";

            foreach (Playlist pl in Playlists)
            {
                MenuItem OnePlaylistItem = new MenuItem();
                OnePlaylistItem.Name = $"Playlist_{pl.PlaylistID}";
                OnePlaylistItem.Tag = song;
                OnePlaylistItem.Header = $"{pl.PlaylistName}";
                OnePlaylistItem.Click += AddToPlaylistClick;
                PlaylistItem.Items.Add(OnePlaylistItem);
            }

            MenuItem QueueItem = new MenuItem();
            QueueItem.Name = $"Queue_{CorrectSongID}";
            QueueItem.Header = "Add to Queue";
            QueueItem.Tag = song;
            QueueItem.Click += AddToQueueClick;

            menu.Items.Add(PlaylistItem);
            menu.Items.Add(QueueItem);
            RecommendedSongList.ContextMenu = menu;
        }

        private void AddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            MenuItem SongItem = sender as MenuItem;
            Song song = (Song)SongItem.Tag;
            int PlaylistID = Convert.ToInt32(SongItem.Name.Substring(9));
            int SongID = song.SongID;
            Playlist relevantPlaylist = user.Playlists.Where(i => i.PlaylistID == PlaylistID).FirstOrDefault();
            relevantPlaylist.AddSongToPlaylist(SongID);


        }
        private void AddToQueueClick(object sender, RoutedEventArgs e)
        {
            var SongItem = sender as MenuItem;
            Song song = (Song)SongItem.Tag;
            MusicQueue.AddSongToQueue(song);           
        }

        private void DeleteFromPlaylistClick(object sender, RoutedEventArgs e)
        {
            var SongItem = sender as MenuItem;
            Song song = (Song)SongItem.Tag;
            playlistToUse.DeleteSongFromPlaylist(song.SongID);
            playlistToUse.RefreshPlaylist();
            rerender(playlistToUse);
        }

        public void DeletePlaylistClick(object sender, RoutedEventArgs e)
        {
            var PlaylistButton = sender as Button;
            Playlist playlist = (Playlist)PlaylistButton.Tag;
            playlist.DeletePlaylist();
            playlist.SongPlaylist.Clear();
            mainWindow.RerenderPlaylists();
            rerender(playlist);
        }

        public void RenamePlaylist(object sender, RoutedEventArgs e)
        {
            var PlaylistButton = sender as Button;
            Playlist playlist = (Playlist)PlaylistButton.Tag;
            NewPlaylistWindow RenameWindow = new NewPlaylistWindow(true, playlistToUse);
            RenameWindow.Show();
            RenameWindow.Closed += (object sender2, EventArgs e2) => { playlistToUse.RefreshPlaylist(); rerender(playlistToUse); mainWindow.RerenderPlaylists(); };
        }


        private void PlaySongFromPlaylist(object sender, RoutedEventArgs e)
        {
            mainWindow.audioPlayer.OnButtonStopClick();

            Button _ButtonSong = sender as Button;
            string SongID = _ButtonSong.Name;
            SongID = SongID.Substring(2);

            //Adds the song the the users play history
            db.AddSongToHistory(user.UserID, Int32.Parse(SongID), 10);
            
            Song songObject = (Song)_ButtonSong.Tag;
            mainWindow.audioPlayer.PlayChosenSong(songObject);
        }

        private void PlayPlaylist(object sender, RoutedEventArgs e)
        {
            MusicQueue.SongQueue.Clear();
            MusicQueue.AddPlaylistToQueue(playlistToUse, RecommendedSongs);
            if(MusicQueue.IsShuffle == true)
            {
                MusicQueue.ShuffleSongs();
            }
            mainWindow.audioPlayer.PlayChosenSong();
        }
    }
}
