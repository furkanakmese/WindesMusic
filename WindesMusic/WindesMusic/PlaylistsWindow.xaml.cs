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
        public PlaylistsWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            Database db = new Database();
            User user = db.GetUserData(Properties.Settings.Default.UserID);
            foreach(var item in user.Playlists)
            {
                Button btnPlaylist = new Button();
                btnPlaylist.Height = 30;
                btnPlaylist.Content = item.PlaylistName;
                stackPlaylists.Children.Add(btnPlaylist);
            }
        }
    }
}
