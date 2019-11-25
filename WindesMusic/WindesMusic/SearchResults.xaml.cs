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
        public SearchResults(string input)
        {
            InitializeComponent();

            Database db = new Database();
            stackResults.Children.Clear();
            List<Song> resultList = db.GetSearchResults(input);
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
                    tbTitle.Margin = new Thickness(0, 0, 10, 10);
                    tbTitle.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                    stackResults.Children.Add(tbTitle);

                    bool artistMatched = resultList.Where(item => item.Artist == input).Count() > 0 ? true : false;

                    TextBlock tbArtists = new TextBlock();
                    tbArtists.Text = "Artists";
                    tbArtists.FontSize = 18;
                    tbArtists.FontWeight = FontWeights.Bold;
                    tbArtists.Margin = new Thickness(0, 0, 10, 10);
                    tbArtists.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                    stackResults.Children.Add(tbArtists);

                    if (artistMatched)
                    {
                        Rectangle image = new Rectangle();
                        image.Width = 80;
                        image.Height = 80;
                        image.HorizontalAlignment = HorizontalAlignment.Left;
                        image.Fill = new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);

                        TextBlock label = new TextBlock();
                        label.Text = input;
                        label.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                        label.HorizontalAlignment = HorizontalAlignment.Left;

                        Thickness thick = new Thickness();
                        thick.Top = 10;
                        thick.Bottom = 10;
                        image.Margin = thick;
                        stackResults.Children.Add(image);
                        stackResults.Children.Add(label);
                    }
                    else
                    {
                        TextBlock tbNoArtistsFound = new TextBlock();
                        tbNoArtistsFound.Text = "No artists found";
                        tbNoArtistsFound.FontSize = 14;
                        tbNoArtistsFound.FontWeight = FontWeights.Bold;
                        tbNoArtistsFound.Margin = new Thickness(0, 0, 10, 10);
                        tbNoArtistsFound.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                        stackResults.Children.Add(tbNoArtistsFound);
                    }

                    TextBlock tbSongsTitle = new TextBlock();
                    tbSongsTitle.Text = "Songs";
                    tbSongsTitle.FontSize = 18;
                    tbSongsTitle.FontWeight = FontWeights.Bold;
                    tbSongsTitle.Margin = new Thickness(0, 0, 0, 10);
                    tbSongsTitle.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                    stackResults.Children.Add(tbSongsTitle);

                    Grid grid = new Grid();
                    grid.Width = 780;
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    grid.VerticalAlignment = VerticalAlignment.Top;

                    foreach (var item in resultList)
                    {
                        StackPanel songPanel = new StackPanel();
                        songPanel.Orientation = Orientation.Horizontal;
                        songPanel.Margin = new Thickness(0, 0, 0, 12);

                        TextBlock tbSongName = new TextBlock();
                        tbSongName.Text = item.SongName;
                        tbSongName.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                        tbSongName.FontSize = 14;
                        tbSongName.Margin = new Thickness(0, 0, 0, 0);
                        songPanel.Children.Add(tbSongName);

                        TextBlock tbSongArtist = new TextBlock();
                        tbSongArtist.Text = item.Artist;
                        tbSongArtist.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                        tbSongArtist.FontSize = 14;
                        tbSongArtist.Margin = new Thickness(50, 0, 0, 0);
                        songPanel.Children.Add(tbSongArtist);

                        TextBlock tbSongAlbum = new TextBlock();
                        tbSongAlbum.Text = item.Album;
                        tbSongAlbum.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                        tbSongAlbum.FontSize = 14;
                        tbSongAlbum.Margin = new Thickness(50, 0, 0, 0);
                        songPanel.Children.Add(tbSongAlbum);

                        stackResults.Children.Add(songPanel);
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
    }
}
