﻿using System;
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
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Page
    {
        Database db = new Database();
        User user;
        public delegate void Logout();
        public event Logout logout;

        public Account()
        {
            InitializeComponent();

            user = db.GetUserData(Properties.Settings.Default.UserID);
            lblName.Text = (user.IsArtist == 1 ? "Artist: " : "User: ") + user.Name;
            btnRequestArtistStatus.Visibility = user.IsArtist == 1 ? Visibility.Hidden : Visibility.Visible;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.UserID = 0;
            Properties.Settings.Default.Save();
            if(logout != null)
            {
                logout();
            }
        }

        private void btnRequestArtistStatus_Click(object sender, RoutedEventArgs e)
        {
            db.RequestArtistStatus();
            lblMessage.Text = "You are now an artist";
        }
    }
}