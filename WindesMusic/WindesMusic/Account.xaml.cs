using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Page
    {
        Database db = new Database();
        User user;
        public delegate void Logout();
        public event Logout logout;
        private MainWindow mainWindow;

        public Account(MainWindow mainWindow, Playlists playlists)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            user = db.GetUserData(Properties.Settings.Default.UserID);
            lblName.Text = (user.IsArtist == true ? "Artist: " : "User: ") + user.Name;
            btnRequestArtistStatus.Visibility = user.IsArtist == true ? Visibility.Hidden : Visibility.Visible;

            lblRequestAd.Text = user.IsArtist == true ? "Request song for advertising" : "";
            btnSubmit.Visibility = user.IsArtist == true ? Visibility.Visible : Visibility.Hidden;

            foreach (var item in user.Songs)
            {
                boxSongs.Items.Add(item.SongName);
            }
            foreach (var item in db.GetAllArtists())
            {
                boxArtists.Items.Add(item);
            }
        }

        private void btnStatisticsClick(object sender, RoutedEventArgs e)
        {
            UserStatistics userStatistics = new UserStatistics();
            userStatistics.Show();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.UserID = 0;
            Properties.Settings.Default.Save();
            if (logout != null)
            {
                logout();
            }
        }

        private void btnRequestArtistStatus_Click(object sender, RoutedEventArgs e)
        {
            db.RequestArtistStatus();
            lblMessage.Text = "You are now an artist";
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (boxSongs.SelectedItem != null)
            {
                string submitAdvertisement = db.SubmitSongForAdvertising(user.Songs.Where(i => i.SongName.Equals(boxSongs.SelectedItem.ToString())).Select(i => i.SongID).First(), user.UserID);
                lblMessage.Text = submitAdvertisement;
            }
            else
            {
                lblMessage.Text = "Please select a song";
            }
        }

        private void btnSubmitCredits_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = db.DonateCredits(user.UserID, boxArtists.Text, Convert.ToInt32(inputCredits.Text));
                lblMessage.Text = result;
            } catch(Exception)
            {
                lblMessage.Text = "Please select an amount";
            }
        }
    }
}
