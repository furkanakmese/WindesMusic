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
    /// Interaction logic for SearchResults.xaml
    /// </summary>
    public partial class SearchResults : Page
    {
        List<Song> resultList;
        User user;
        MainWindow mainWindow;

        public SearchResults(string input, User us, MainWindow main)
        {
            InitializeComponent();

            mainWindow = main;
            user = us;
            Thickness SongBlockThickness = new Thickness(5, 2, 0, 0);
            SolidColorBrush whiteText = new SolidColorBrush(System.Windows.Media.Colors.White);
            Database db = new Database();
            stackResults.Children.Clear();
            resultList = db.GetSearchResults(input);
            TextBlock tbMessage = new TextBlock();
            tbMessage.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
            tbMessage.FontSize = 20;

            if (resultList.Count > 0)
            {
                if (input.Trim() != "" && !input.Trim().Contains("_"))
                {
                    // here it shows the search results, shown like the screen design in FO
                    TextBlock tbTitle = new TextBlock();
                    tbTitle.Text = "Top results for " + input;
                    tbTitle.FontSize = 18;
                    tbTitle.FontWeight = FontWeights.Bold;
                    tbTitle.Margin = new Thickness(10, 5, 10, 10);
                    tbTitle.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                    stackResults.Children.Add(tbTitle);

                    bool artistMatched = resultList.Where(item => item.Artist == input).Count() > 0 ? true : false;

                    TextBlock tbArtists = new TextBlock();
                    tbArtists.Text = "Artists";
                    tbArtists.FontSize = 18;
                    tbArtists.FontWeight = FontWeights.Bold;
                    tbArtists.Margin = new Thickness(10, 0, 10, 10);
                    tbArtists.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                    stackResults.Children.Add(tbArtists);

                    if (artistMatched)
                    {
                        if (input == "Metallica")
                        {
                            Image image = new Image();
                            image.Width = 100;
                            image.Height = 100;
                            image.HorizontalAlignment = HorizontalAlignment.Left;
                            image.Stretch = Stretch.UniformToFill;
                            image.Source = new BitmapImage(new Uri(@"https://upload.wikimedia.org/wikipedia/commons/0/07/Metallica_at_The_O2_Arena_London_2008.jpg"));

                            TextBlock label = new TextBlock();
                            label.Text = input;
                            label.FontSize = 15;
                            label.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                            label.HorizontalAlignment = HorizontalAlignment.Left;

                            image.Margin = new Thickness(10, 5, 0, 0);
                            label.Margin = new Thickness(10, 5, 0, 0);
                            stackResults.Children.Add(image);
                            stackResults.Children.Add(label);
                            SearchScroller.Margin = new Thickness(-10, 200, 0, 0);
                            SearchScroller.Height = 400;
                        }
                        else
                        {
                            Rectangle image = new Rectangle();
                            image.Width = 100;
                            image.Height = 100;
                            image.HorizontalAlignment = HorizontalAlignment.Left;
                            image.Fill = new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);

                            TextBlock label = new TextBlock();
                            label.Text = input;
                            label.FontSize = 15;
                            label.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                            label.HorizontalAlignment = HorizontalAlignment.Left;

                            image.Margin = new Thickness(10, 5, 0, 0);
                            label.Margin = new Thickness(10, 5, 0, 0);
                            stackResults.Children.Add(image);
                            stackResults.Children.Add(label);
                            SearchScroller.Margin = new Thickness(-10, 200, 0, 0);
                            SearchScroller.Height = 400;
                        }
                    }
                    else
                    {
                        TextBlock tbNoArtistsFound = new TextBlock();
                        tbNoArtistsFound.Text = "No artists found";
                        tbNoArtistsFound.FontSize = 14;
                        tbNoArtistsFound.FontWeight = FontWeights.Bold;
                        tbNoArtistsFound.Margin = new Thickness(10, 0, 10, 10);
                        tbNoArtistsFound.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                        stackResults.Children.Add(tbNoArtistsFound);
                    }

                    for (int i = 0; i < resultList.Count; i++)
                    {
                        Song playlistSong = resultList[i];
                        RowDefinition rowDef = new RowDefinition();
                        rowDef.Name = $"Row_{i}";
                        SearchList.RowDefinitions.Add(rowDef);
                        RowDefinitionCollection RowNames = SearchList.RowDefinitions;
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
                        PlayButton.Click += PlaySongFromSearch;

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
                        SearchList.Children.Add(PlayButton);
                        SearchList.Children.Add(SongBlockName);
                        SearchList.Children.Add(SongBlockArtist);
                        SearchList.Children.Add(SongBlockAlbum);
                        SearchList.Children.Add(SongBlockYear);

                        ContextMenu menu = new ContextMenu();
                        menu.Background = new SolidColorBrush(System.Windows.Media.Colors.Black);
                        menu.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);

                        SearchList.MouseRightButtonDown += new MouseButtonEventHandler(SongContextMenuOpening);
                    }

                    
                }
                else
                {
                    tbMessage.Text = "Please type in search criteria";
                    stackResults.Children.Add(tbMessage);
                }
            }
            else
            {
                tbMessage.Text = "No results found";
                stackResults.Children.Add(tbMessage);
            }
        }

        private void PlaySongFromSearch(object sender, RoutedEventArgs e)
        {
            Button _PlayButton = sender as Button;
            Song song = (Song)_PlayButton.Tag;
            mainWindow.audioPlayer.PlayChosenSong(song);
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

        private void SongContextMenuOpening(object sender, MouseButtonEventArgs e)
        {
            List<Playlist> Playlists = user.Playlists;
            var pos = e.GetPosition(SearchList);
            double top = pos.Y;
            int top1 = (int)Math.Round(top);
            int amount = top1 / 28;
            ContextMenu menu = new ContextMenu();
            Song song = resultList.ElementAt(amount);
            int CorrectSongID = resultList.ElementAt(amount).SongID;

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
            QueueItem.Tag= song;
            QueueItem.Header = "Add to Queue";
            QueueItem.Click += AddToQueueClick;

            menu.Items.Add(PlaylistItem);
            menu.Items.Add(QueueItem);
            SearchList.ContextMenu = menu;
        }
    }
}
