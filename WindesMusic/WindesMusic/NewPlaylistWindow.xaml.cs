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
    /// Interaction logic for NewPlaylistWindow.xaml
    /// </summary>
    public partial class NewPlaylistWindow : Window
    {
        public bool IsRename = false;
        public Playlist playlist;
        public NewPlaylistWindow()
        {
            InitializeComponent();
            if(IsRename == true)
            {
                TextTop.Content = "Rename playlist";
            }
        }

        public NewPlaylistWindow(bool isRename, Playlist pl)
        {
            InitializeComponent();
            IsRename = isRename;
            if (IsRename == true)
            {
                TextTop.Content = "Rename playlist";
                CreateNewPlaylistButton.Content = "Rename playlist";
                playlist = pl;
            }
        }

        private void MakeNewPlaylistButton(object sender, RoutedEventArgs e)
        {
            string input = InputName.Text;

            if (IsRename == false)
            {
                if (input.Trim() != "" && !input.Trim().Contains("_"))
                {
                    Database data = new Database();
                    string PlaylistName = input;
                    data.CreateNewPlaylist(PlaylistName, WindesMusic.Properties.Settings.Default.UserID);
                    this.Close();
                }
                else
                {
                    NewPlaylistMessage.Text = "Please use a valid name";
                }
            }
            else
            {
                if (input.Trim() != "" && !input.Trim().Contains("_"))
                {
                    string PlaylistName = input;
                    playlist.RenamePlaylist(input);
                    this.Close();
                }
                else
                {
                    NewPlaylistMessage.Text = "Please use a valid name";
                }
            }
        }
    }
}
