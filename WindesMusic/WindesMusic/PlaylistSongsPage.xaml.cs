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
        List<Song> SongsInPlaylist;
        List<Song> RecommendedSongs = new List<Song>();
        MainWindow mainWindow;
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
            RecommendedSongList.Children.Clear();

            Recommender recommender = new Recommender(db);
            RecommendedSongs = recommender.getRecommendedSongsForPlaylist(playlist);
            playlistToUse = playlist;
            mainWindow = main;
            user = BaseUser;
            _PlaylistName = playlistToUse.PlaylistName;
            _PlaylistID = playlistToUse.PlaylistID;
            user = BaseUser;
            if (SongsInPlaylist == null)
            {
                SongsInPlaylist = playlistToUse.SongPlaylist;
            }
            Thickness SongBlockThickness = new Thickness(5, 2, 0, 0);
            SolidColorBrush whiteText = new SolidColorBrush(System.Windows.Media.Colors.White);
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
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
                Margin = new Thickness(30, 10, 0, 5)
            };
            PlayPlaylistButton.Click += PlayPlaylist;

            sp.Children.Add(PlaylistBlock);
            sp.Children.Add(PlayPlaylistButton);
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
            for (int i = 0; i < playlistToUse.SongPlaylist.Count; i++)
            {
                Song playlistSong = playlistToUse.SongPlaylist[i];
                RowDefinition rowDef = new RowDefinition();
                rowDef.Name = $"Row_{i}";
                SongList.RowDefinitions.Add(rowDef);
                RowDefinitionCollection RowNames = SongList.RowDefinitions;
                Array RowArray = RowNames.ToArray();

                // Add the play button to the Songlist grid
                var PlayButton = new Button
                {
                    Name = $"__{playlistSong.SongID}",
                    Content = "Play",
                    Margin = new Thickness(5, 0, 0, 5),
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

                ContextMenu menu = new ContextMenu();
                menu.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
                menu.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);

                SongList.ContextMenu = null;
                SongList.MouseRightButtonDown += new MouseButtonEventHandler(SongContextMenuOpening);
            }

            for (int i = 0; i < RecommendedSongs.Count; i++)
            {
                Song playlistSong = RecommendedSongs[i];
                RowDefinition rowDef = new RowDefinition();
                rowDef.Name = $"Row_{i}";
                RecommendedSongList.RowDefinitions.Add(rowDef);
                //Song playlistSong = PlaylistSongs[i];
                RowDefinitionCollection RowNames = RecommendedSongList.RowDefinitions;
                Array RowArray = RowNames.ToArray();

                // Add the play button to the Songlist grid
                var PlayButton = new Button
                {
                    Name = $"__{playlistSong.SongID}",
                    Content = "Play",
                    Margin = new Thickness(5, 0, 0, 5),
                    FontSize = 15
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
                RecommendedSongList.Children.Add(PlayButton);
                RecommendedSongList.Children.Add(SongBlockName);
                RecommendedSongList.Children.Add(SongBlockArtist);
                RecommendedSongList.Children.Add(SongBlockAlbum);
                RecommendedSongList.Children.Add(SongBlockYear);

                RecommendedSongList.MouseRightButtonDown += new MouseButtonEventHandler(SongContextMenuFromRecommended);
            }
            
        }
        private void OnLabelClick(object sender, EventArgs args)
        {
            Playlist newPlaylist = new Playlist();
            newPlaylist.PlaylistID = playlistToUse.PlaylistID;
            newPlaylist.PlaylistName = playlistToUse.PlaylistName;
            newPlaylist.Recommender = playlistToUse.Recommender;

            switch (_orderBy)
            {
                case "name":
                    newPlaylist.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.SongName).ToList();
                    break;
                case "album":
                    newPlaylist.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.Album).ToList();
                    break;
                case "year":
                    newPlaylist.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.Year).ToList();
                    break;
                case "artist":
                    newPlaylist.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.Artist).ToList();
                    break;
                default:
                    newPlaylist.SongPlaylist = playlistToUse.SongPlaylist.OrderBy(x => x.SongID).ToList();
                    break;
            }
            if (rerender != null)
            {
                rerender(newPlaylist);
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
            Song song = SongsInPlaylist.ElementAt(amount);
            int CorrectSongID = SongsInPlaylist.ElementAt(amount).SongID;

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
            Song song = SongsInPlaylist.ElementAt(amount);
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
            SongList.ContextMenu = menu;
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


        private void PlaySongFromPlaylist(object sender, RoutedEventArgs e)
        {
            mainWindow.audioPlayer.OnButtonStopClick();

            Button _ButtonSong = sender as Button;
            Song songObject = (Song)_ButtonSong.Tag;
            mainWindow.audioPlayer.PlayChosenSong(songObject);
        }

        private void PlayPlaylist(object sender, RoutedEventArgs e)
        {
            MusicQueue.SongQueue.Clear();
            MusicQueue.SongQueue = playlistToUse.CreateQueueFromPlaylist();
            if(MusicQueue.IsShuffle == true)
            {
                MusicQueue.ShuffleSongs();
            }
            mainWindow.audioPlayer.PlayChosenSong();
        }
    }
}