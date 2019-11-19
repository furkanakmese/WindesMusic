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
using System.Windows.Shapes;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for PlaylistsWindow.xaml
    /// </summary>
    public partial class PlaylistsWindow : Window
    {
        public Database db = new Database();
        public PlaylistsWindow()
        {
            InitializeComponent();

            // Properties.Settings.Default.UserID = 0;
            // Properties.Settings.Default.Save();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            User user = db.GetUserData(Properties.Settings.Default.UserID);

            foreach(var item in user.Playlists)
            {
                Button btnPlaylist = new Button();
                btnPlaylist.Height = 30;
                btnPlaylist.Content = item.PlaylistName;
                stackPlaylists.Children.Add(btnPlaylist);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            stackPlaylists.Children.Clear();
            List<Song> resultList = db.GetSearchResults(inputSearch.Text);
            if(resultList.Count > 0)
            {
                if(inputSearch.Text.Trim() != "" && !inputSearch.Text.Trim().Contains("_"))
                {
                    foreach (var item in resultList)
                    {
                        Button btnPlaylist = new Button();
                        btnPlaylist.Height = 30;
                        btnPlaylist.Content = item.SongName;
                        stackPlaylists.Children.Add(btnPlaylist);
                    }
                } else
                {
                    TextBlock tbEmptySearch = new TextBlock();
                    tbEmptySearch.Text = "Please type in search criteria";
                    stackPlaylists.Children.Add(tbEmptySearch);
                }
            } else
            {
                TextBlock tbNoResults = new TextBlock();
                tbNoResults.Text = "No results found";
                stackPlaylists.Children.Add(tbNoResults);
            }
        }
    }
}
