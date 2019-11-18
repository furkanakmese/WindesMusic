using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Database database1 = new Database();
            List<int> PlaylistIDs = new List<int>();
            List<string> PlaylistNames = new List<string>();
            PlaylistList.Children.Clear();
            PlaylistIDs = database1.GetRecordsInt($"SELECT PlaylistID FROM Playlist WHERE UserID = 1", "PlaylistID");
            PlaylistNames = database1.GetRecordsString($"SELECT Name FROM Playlist WHERE UserID = 1", "Name");
            Thickness thickness = new Thickness(25, 0, 0, 5);
            Style style = new Style();
            
            for(int i = 0; i < PlaylistIDs.Count; i++)
            {
                var PlaylistButton = new Button
                {
                    //Style = StaticResource MenuButton,
                    Name = $"_{PlaylistIDs[i]}",
                    Content = $"{PlaylistNames[i]}",
                    Margin = thickness
                };
                PlaylistButton.Click += ButtonClickPlaylist;
                PlaylistList.Children.Add(PlaylistButton);
            }
        }

        private void ButtonClickPlaylist(object sender, RoutedEventArgs e)
        {
            SongList.Children.Clear();
            Database data = new Database();
            Button _ButtonPlaylist = sender as Button;
            string PlaylistIDName = _ButtonPlaylist.Name;
            PlaylistIDName = PlaylistIDName.Substring(1);
            int PlaylistID = Convert.ToInt32(PlaylistIDName);

            List<string> SongNames = new List<string>();
            SongNames = data.GetRecordsString($"SELECT Name FROM Song WHERE SongID IN(SELECT SongID FROM PlaylistToSong WHERE PlaylistID = {PlaylistID})", "Name");
            Playlist playlist = new Playlist(PlaylistID);
            Thickness thickness = new Thickness(25, 0, 0, 5);
            for(int i = 0; i < playlist.SongPlaylist.Count; i++)
            {
                var SongBlock = new TextBlock
                {
                    Name = $"_{playlist.SongPlaylist[i]}",
                    Text = $"{SongNames[i]}",
                    Margin = thickness
                };
                SongList.Children.Add(SongBlock);
            }
        }
        
    }
}