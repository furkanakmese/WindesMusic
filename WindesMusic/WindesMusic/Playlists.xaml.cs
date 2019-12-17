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
        MainWindow mainWindow;
        User user;

        public Playlists(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;

            Database db = new Database();
            user = db.GetUserData(Properties.Settings.Default.UserID);

            foreach (var item in user.Playlists)
            {
                Rectangle image = new Rectangle();
                image.Width = 150;
                image.Height = 150;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Fill = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue);
                image.Tag = item;
                image.MouseLeftButtonDown += PlaylistClickRectangle;

                TextBlock label = new TextBlock();
                label.Text = item.PlaylistName;
                label.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Tag = item;
                label.MouseLeftButtonDown += PlaylistClickTextBlock;
                

                Thickness thick = new Thickness();
                thick.Top = 10;
                thick.Bottom = 10;
                image.Margin = thick;
                stackPlaylists.Children.Add(image);
                stackPlaylists.Children.Add(label);
            }
        }

        private void PlaylistClickTextBlock(object sender, RoutedEventArgs e)
        {
            TextBlock label = sender as TextBlock;
            Playlist playlist = (Playlist)label.Tag;
            mainWindow.playlistSongs.reinitialize(playlist, mainWindow, user);
            mainWindow.Main.Content = mainWindow.playlistSongs;
        }
        private void PlaylistClickRectangle(object sender, RoutedEventArgs e)
        {
            Rectangle label = sender as Rectangle;
            Playlist playlist = (Playlist)label.Tag;
            mainWindow.playlistSongs.reinitialize(playlist, mainWindow, user);
            mainWindow.Main.Content = mainWindow.playlistSongs;
        }
    }
}
