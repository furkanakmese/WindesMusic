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
    /// Interaction logic for QueuePage.xaml
    /// </summary>
    public partial class QueuePage : Page
    {
        List<Song> songs;
        MainWindow mainWindow;

        public delegate void OnRerender(QueuePage queuePage);
        public event OnRerender rerender;

        public QueuePage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
        }
        public void InitialiseQueuePage()
        {
            InitializeComponent();
            QueueList.Children.Clear();
            songs = new List<Song>(MusicQueue.SongQueue);

            Thickness SongBlockThickness = new Thickness(5, 2, 0, 0);
            SolidColorBrush whiteText = new SolidColorBrush(System.Windows.Media.Colors.White);

            for (int i = 0; i < songs.Count; i++)
            {
                Song song = songs[i];
                RowDefinition rowDef = new RowDefinition();
                rowDef.Name = $"Row_{i}";
                QueueList.RowDefinitions.Add(rowDef);
                RowDefinitionCollection RowNames = QueueList.RowDefinitions;
                Array RowArray = RowNames.ToArray();

                // Add the play button to the Songlist grid
                var PlayButton = new Button
                {
                    Name = $"__{song.SongID}",
                    Content = "Play",
                    Margin = new Thickness(5, 0, 0, 5),
                    FontSize = 15,
                    Tag = song
                };
                Grid.SetRow(PlayButton, i);
                Grid.SetColumn(PlayButton, 0);
                PlayButton.Click += PlaySongFromQueue;

                // Add the Songname text block to the Songlist grid
                var SongBlockName = new TextBlock
                {
                    Name = $"_{song.SongID}",
                    Text = $"{song.SongName}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockName, i);
                Grid.SetColumn(SongBlockName, 1);

                // Add the artist text block to the Songlist grid
                var SongBlockArtist = new TextBlock
                {
                    Name = $"_{song.SongID}",
                    Text = $"{song.Artist}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockArtist, i);
                Grid.SetColumn(SongBlockArtist, 2);

                // Add the album text block to the Songlist grid
                var SongBlockAlbum = new TextBlock
                {
                    Name = $"_{song.SongID}",
                    Text = $"{song.Album}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockAlbum, i);
                Grid.SetColumn(SongBlockAlbum, 3);

                // Add the year text block to the Songlist grid
                var SongBlockYear = new TextBlock
                {
                    Name = $"_{song.SongID}",
                    Text = $"{song.Year}",
                    Foreground = whiteText,
                    Margin = SongBlockThickness,
                    FontSize = 15
                };
                Grid.SetRow(SongBlockYear, i);
                Grid.SetColumn(SongBlockYear, 4);

                // Add the elements to the Songlist grid Children collection
                QueueList.Children.Add(PlayButton);
                QueueList.Children.Add(SongBlockName);
                QueueList.Children.Add(SongBlockArtist);
                QueueList.Children.Add(SongBlockAlbum);
                QueueList.Children.Add(SongBlockYear);
                QueueToPlaylist.MouseLeftButtonUp += AddQueueToPlaylist;
            }

            }
        private void PlaySongFromQueue(object sender, RoutedEventArgs e)
        {
            mainWindow.audioPlayer.OnButtonStopClick();

            Button _ButtonSong = sender as Button;
            Song songObject = (Song)_ButtonSong.Tag;
            mainWindow.audioPlayer.PlayChosenSong(songObject);
        }

        
        private void AddQueueToPlaylist(object sender, RoutedEventArgs e)
        {
            List<Playlist> Playlists = mainWindow.user.Playlists;
            ContextMenu menu = new ContextMenu();
            menu.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
            menu.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);

            foreach (Playlist pl in Playlists)
            {
                MenuItem OnePlaylistItem = new MenuItem();
                OnePlaylistItem.Name = $"Playlist_{pl.PlaylistID}";
                OnePlaylistItem.Tag = pl;
                OnePlaylistItem.Header = $"{pl.PlaylistName}";
                OnePlaylistItem.Click += AddToPlaylistClick;
                OnePlaylistItem.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
                OnePlaylistItem.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                menu.Items.Add(OnePlaylistItem);
            }
            QueueToPlaylist.ContextMenu = menu;
            QueueToPlaylist.ContextMenu.IsOpen = true;
        }

        private void AddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            MenuItem PlaylistItem = sender as MenuItem;
            Playlist playlist = (Playlist)PlaylistItem.Tag;
            foreach(Song s in songs)
            {
                playlist.AddSongToPlaylist(s);
            }
        }

        public void Initialise(object sender, RoutedEventArgs e)
        {
            this.InitialiseQueuePage();
        }

        public void RerenderQueuePage()
        {
            if (rerender != null)
            {
                rerender(this);
            }
        }
    }
}
