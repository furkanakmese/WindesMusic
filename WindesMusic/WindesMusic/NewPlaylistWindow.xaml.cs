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
        public NewPlaylistWindow()
        {
            InitializeComponent();
        }

        private void MakeNewPlaylistButton(object sender, RoutedEventArgs e)
        {
            Database data = new Database();
            string PlaylistName = InputName.Text;
            data.SetValues($"INSERT INTO Playlist(PlaylistName, UserID) VALUES ('{PlaylistName}', {WindesMusic.Properties.Settings.Default.UserID})");       
            this.Close();
        }
    }
}
