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
        private int _PlaylistID;
        private string _PlaylistName;
        List<Song> SongsInPlaylist;
        MainWindow mainWindow;
        Recommender recommender;
        public PlaylistSongsPage(Playlist playlist, MainWindow main)
        {
            InitializeComponent();
            playlist.Recommender = new Recommender();
            playlist.Recommender.getRecommendedSongsForPlaylist(playlist.SongPlaylist, db);
            SongsInPlaylist = playlist.SongPlaylist;
            mainWindow = main;
            _PlaylistName = playlist.PlaylistName;
            _PlaylistID = playlist.PlaylistID;
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
                Name = $"_{_PlaylistID}",
                Content = "Play",
                FontSize = 30,
                Margin = new Thickness(250, 10, 260, 5)
            };
            PlayPlaylistButton.Click += PlayPlaylist;

            PlaylistName.Children.Add(PlaylistBlock);
            PlaylistName.Children.Add(PlayPlaylistButton);

            //Adds the necessary amount of rows for the playlist
            for (int i = 0; i < playlist.SongPlaylist.Count; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                SongList.RowDefinitions.Add(rowDef);
            }

            for (int i = 0; i < playlist.SongPlaylist.Count; i++)
            {
                Song playlistSong = playlist.SongPlaylist[i];

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
                SongList.Children.Add(PlayButton);
                SongList.Children.Add(SongBlockName);
                SongList.Children.Add(SongBlockArtist);
                SongList.Children.Add(SongBlockAlbum);
                SongList.Children.Add(SongBlockYear);

                ContextMenu menu = new ContextMenu();
                menu.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
                menu.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                MenuItem PlaylistItem = new MenuItem();
                PlaylistItem.Name = $"Playlist_{playlistSong.SongID}";
                PlaylistItem.Header = "Add to Playlist";
                PlaylistItem.Click += AddToPlaylistClick;

                MenuItem QueueItem = new MenuItem();
                PlaylistItem.Name = $"Queue_{playlistSong.SongID}";
                QueueItem.Header = "Add to Queue";
                QueueItem.Click += AddToQueueClick;

                menu.Items.Add(PlaylistItem);
                menu.Items.Add(QueueItem);
                SongList.ContextMenu = menu;
            }
        }

        private void AddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            MenuItem Song = sender as MenuItem;
            int SongID = Convert.ToInt32(Song.Name.Substring(9));
            
        }
        private void AddToQueueClick(object sender, RoutedEventArgs e)
        {

        }


        private void PlaySongFromPlaylist(object sender, RoutedEventArgs e)
        {
            mainWindow.audioPlayer.OnButtonStopClick();

            Button _ButtonSong = sender as Button;
            string SongID = _ButtonSong.Name;
            SongID = SongID.Substring(2);
            mainWindow.audioPlayer.PlayChosenSong(SongID);
        }

        private void PlayPlaylist(object sender, RoutedEventArgs e)
        {
            Playlist playlist = new Playlist(_PlaylistID);
            playlist.SongPlaylist = db.GetSongsInPlaylist(_PlaylistID);
            MusicQueue.SongQueue = playlist.CreateQueueFromPlaylist();
            mainWindow.audioPlayer.PlayChosenSong();
        }
    }
}