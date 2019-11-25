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
        public PlaylistSongsPage(int playlistId, string NameOfPlaylist, List<Song> PlaylistSongs, MainWindow main)
        {
            InitializeComponent();
            SongsInPlaylist = PlaylistSongs;
            mainWindow = main;
            _PlaylistName = NameOfPlaylist;
            _PlaylistID = playlistId;
            Thickness thickness = new Thickness(10, 2, 0, 5);
            Thickness thickness2 = new Thickness(10, 0, 0, 5);
            Thickness thickness3 = new Thickness(250, 10, 260, 5);
            Thickness thickness4 = new Thickness(250, 10, 0, 5);
            SolidColorBrush whiteText = new SolidColorBrush(System.Windows.Media.Colors.White);
            StackPanel spPlaylist = new StackPanel();
            spPlaylist.Orientation = Orientation.Horizontal;
            spPlaylist.VerticalAlignment = VerticalAlignment.Stretch;
            spPlaylist.HorizontalAlignment = HorizontalAlignment.Stretch;
            var PlaylistBlock = new TextBlock
            {
                Text = $"{NameOfPlaylist}",
                FontSize = 25,
                Foreground = whiteText,
                Margin = thickness4
            };
            var PlayPlaylistButton = new Button
            {
                Name = $"_{_PlaylistID}",
                Content = "Play",
                FontSize = 30,
                Margin = thickness3
            };
            PlayPlaylistButton.Click += PlayPlaylist;

            //spPlaylist.Children.Add(PlaylistBlock);
            //spPlaylist.Children.Add(PlayPlaylistButton);
            PlaylistName.Children.Add(PlaylistBlock);
            PlaylistName.Children.Add(PlayPlaylistButton);
            foreach (Song playlistSong in PlaylistSongs)
            {
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
                    Foreground = whiteText,
                    Margin = thickness
                };
                var SongBlockArtist = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Artist}",
                    Foreground = whiteText,
                    Margin = thickness
                };
                var SongBlockAlbum = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Album}",
                    Foreground = whiteText,
                    Margin = thickness
                };
                var SongBlockYear = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Year}",
                    Foreground = whiteText,
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
