﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // set initial screen based on previous login
            if (WindesMusic.Properties.Settings.Default.UserID == 0)
            {
                LoginWindow login = new LoginWindow();
                login.Show();
            } else
            {
                MainWindow main = new MainWindow();
                main.Show();
            }
        }
    }
}
