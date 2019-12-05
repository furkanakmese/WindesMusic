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
    /// Interaction logic for Playlists.xaml
    /// </summary>
    public partial class Playlists : Page
    {
        public Playlists()
        {
            InitializeComponent();

            Database db = new Database();
            User user = db.GetUserData(Properties.Settings.Default.UserID);

            foreach (var item in user.Playlists)
            {
                Rectangle image = new Rectangle();
                image.Width = 150;
                image.Height = 150;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Fill = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue);

                TextBlock label = new TextBlock();
                label.Text = item.PlaylistName;
                label.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                label.HorizontalAlignment = HorizontalAlignment.Left;

                Thickness thick = new Thickness();
                thick.Top = 10;
                thick.Bottom = 10;
                image.Margin = thick;
                stackPlaylists.Children.Add(image);
                stackPlaylists.Children.Add(label);
            }
        }
    }
}
