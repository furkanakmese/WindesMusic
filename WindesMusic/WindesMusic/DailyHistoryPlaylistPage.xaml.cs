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
    /// Interaction logic for DailyHistoryPlaylistPage.xaml
    /// </summary>
    public partial class DailyHistoryPlaylistPage : Page
    {
        private Database db = new Database();
        private string _PlaylistName;
        private int _PlaylistID;
        private MainWindow mainWindow;
        private User user;
        private List<Song> SongsInPlaylist;
        private Playlist playlist;
        public DailyHistoryPlaylistPage(User BaseUser, MainWindow main)
        {
            InitializeComponent();
            user = BaseUser;
            mainWindow = main;
            Recommender recommender = new Recommender(db);
            playlist = recommender.getHistoryPlaylist(user.Id);
            SongsInPlaylist = playlist.SongPlaylist;
            _PlaylistID = playlist.PlaylistID;
            _PlaylistName = playlist.PlaylistName;
            Console.WriteLine(_PlaylistName);
            SongsInPlaylist = playlist.SongPlaylist;
            Thickness SongBlockThickness = new Thickness(5, 2, 0, 0);
            SolidColorBrush whiteText = new SolidColorBrush(System.Windows.Media.Colors.White);

        var PlaylistBlock = new TextBlock
        {
            Text = $"{playlist.PlaylistName}",
            FontSize = 25,
            Foreground = whiteText,
            Margin = new Thickness(250, 10, 0, 5)
        };
        var PlayPlaylistButton = new Button
        {
            Name = $"_{playlist.PlaylistID}",
            Content = "Play",
            FontSize = 30,
            Margin = new Thickness(250, 10, 260, 5)
        };
        PlayPlaylistButton.Click += PlayPlaylist;

            HistoryPlaylistName.Children.Add(PlaylistBlock);
            HistoryPlaylistName.Children.Add(PlayPlaylistButton);

            //Adds the necessary amount of rows for the playlist
            for (int i = 0; i < playlist.SongPlaylist.Count; i++)
            {
                Song playlistSong = playlist.SongPlaylist[i];
                RowDefinition rowDef = new RowDefinition();
                rowDef.Name = $"Row_{i}";
                HistorySongList.RowDefinitions.Add(rowDef);
                //Song playlistSong = PlaylistSongs[i];
                RowDefinitionCollection RowNames = HistorySongList.RowDefinitions;
                Array RowArray = RowNames.ToArray();

                // Add the play button to the Songlist grid
                var PlayButton = new Button
                {
                    Name = $"__{playlistSong.SongID}",
                    Content = "Play",
                    Margin = new Thickness(5, 0, 0, 5)
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
                    Margin = SongBlockThickness
                };
                Grid.SetRow(SongBlockName, i);
                Grid.SetColumn(SongBlockName, 1);

                // Add the artist text block to the Songlist grid
                var SongBlockArtist = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Artist}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness
                };
                Grid.SetRow(SongBlockArtist, i);
                Grid.SetColumn(SongBlockArtist, 2);

                // Add the album text block to the Songlist grid
                var SongBlockAlbum = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Album}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness
                };
                Grid.SetRow(SongBlockAlbum, i);
                Grid.SetColumn(SongBlockAlbum, 3);

                // Add the year text block to the Songlist grid
                var SongBlockYear = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Year}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness
                };
                Grid.SetRow(SongBlockYear, i);
                Grid.SetColumn(SongBlockYear, 4);

                // Add the elements to the Songlist grid Children collection
                HistorySongList.Children.Add(PlayButton);
                HistorySongList.Children.Add(SongBlockName);
                HistorySongList.Children.Add(SongBlockArtist);
                HistorySongList.Children.Add(SongBlockAlbum);
                HistorySongList.Children.Add(SongBlockYear);

                ContextMenu menu = new ContextMenu();
                menu.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
                menu.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);

                HistorySongList.MouseRightButtonDown += new MouseButtonEventHandler(SongContextMenuOpening);
            }


    }
        private void SongContextMenuOpening(object sender, MouseButtonEventArgs e)
        {
            List<Playlist> Playlists = user.Playlists;
            var pos = e.GetPosition(HistorySongList);
            double top = pos.Y;
            int top1 = (int)Math.Round(top);
            int amount = top1 / 25;
            ContextMenu menu = new ContextMenu();
            int CorrectSongID = SongsInPlaylist.ElementAt(amount).SongID;

            MenuItem PlaylistItem = new MenuItem();
            PlaylistItem.Name = $"Playlists";
            PlaylistItem.Header = "Add to Playlist";

            foreach (Playlist pl in Playlists)
            {
                MenuItem OnePlaylistItem = new MenuItem();
                OnePlaylistItem.Name = $"Playlist_{pl.PlaylistID}";
                OnePlaylistItem.Tag = $"{CorrectSongID}";
                OnePlaylistItem.Header = $"{pl.PlaylistName}";
                OnePlaylistItem.Click += AddToPlaylistClick;
                PlaylistItem.Items.Add(OnePlaylistItem);
            }

            MenuItem QueueItem = new MenuItem();
            QueueItem.Name = $"Queue_{CorrectSongID}";
            QueueItem.Header = "Add to Queue";
            QueueItem.Click += AddToQueueClick;

            menu.Items.Add(PlaylistItem);
            menu.Items.Add(QueueItem);
            HistorySongList.ContextMenu = menu;
        }

        private void AddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            MenuItem SongItem = sender as MenuItem;
            int PlaylistID = Convert.ToInt32(SongItem.Name.Substring(9));
            int SongID = Convert.ToInt32(SongItem.Tag);
            Playlist relevantPlaylist = user.Playlists.Where(i => i.PlaylistID == PlaylistID).FirstOrDefault();
            relevantPlaylist.AddSongToPlaylist(SongID);


        }
        private void AddToQueueClick(object sender, RoutedEventArgs e)
        {
            var SongItem = sender as MenuItem;
            int SongID = Convert.ToInt32(SongItem.Name.Substring(6));
            MusicQueue.AddSongToQueue(SongID);
        }


        private void PlaySongFromPlaylist(object sender, RoutedEventArgs e)
        {
            mainWindow.audioPlayer.OnButtonStopClick();

            Button _ButtonSong = sender as Button;
            string SongID = _ButtonSong.Name;
            SongID = SongID.Substring(2);

            //Adds the song the the users play history
            db.AddSongToHistory(user.Id, Int32.Parse(SongID), 10);

            mainWindow.audioPlayer.PlayChosenSong(SongID);
        }

        private void PlayPlaylist(object sender, RoutedEventArgs e)
        {
            MusicQueue.SongQueue.Clear();
            Playlist playlist = new Playlist(_PlaylistID);
            playlist.SongPlaylist = db.GetSongsInPlaylist(_PlaylistID);
            MusicQueue.SongQueue = playlist.CreateQueueFromPlaylist();
            mainWindow.audioPlayer.PlayChosenSong();
        }
    }
}
