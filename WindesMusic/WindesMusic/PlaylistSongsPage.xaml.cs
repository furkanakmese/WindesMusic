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
        List<Song> SongsInPlaylist;
        public PlaylistSongsPage(string NameOfPlaylist, List<Song> PlaylistSongs)
        {
            InitializeComponent();
            SongsInPlaylist = PlaylistSongs;

            Thickness thickness = new Thickness(10, 2, 0, 5);
            Thickness thickness2 = new Thickness(10, 0, 0, 5);
            var PlaylistBlock = new TextBlock
            {
                Text = $"{NameOfPlaylist}",
                Margin = thickness
            };

            PlaylistName.Children.Add(PlaylistBlock);
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
                //PlayButton.Click += PlaySongFromPlaylist;
                var SongBlockName = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.SongName}",
                    Margin = thickness
                };
                var SongBlockArtist = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Artist}",
                    Margin = thickness
                };
                var SongBlockAlbum = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Album}",
                    Margin = thickness
                };
                var SongBlockYear = new TextBlock
                {
                    Name = $"_{playlistSong.SongID}",
                    Text = $"{playlistSong.Year}",
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
        
    }
}
