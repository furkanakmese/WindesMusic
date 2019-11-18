using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public int attempts = 0;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            attempts++;
            lblMessage.Text = "";

            // user tried too many times and has to wait 5 seconds before button works again 
            if (attempts == 5)
            {
                lblMessage.Text = "Too many attempts, wait 5 seconds";
                LoginBtn.IsEnabled = false;
                await Task.Delay(5000);
                LoginBtn.IsEnabled = true;
                attempts = 0;
            } else
            {
                if (InputEmail.Text == "" || InputPassword.Password == "")
                {
                    lblMessage.Text = "Fill in both fields please";
                }
                else
                {
                    Database db = new Database();
                    User resultUser = db.Login(InputEmail.Text, InputPassword.Password);

                    if (resultUser.Email != null)
                    {
                        MainWindow main = new MainWindow();
                        main.Show();
                    }
                    else
                    {
                        lblMessage.Text = "Wrong username or password";
                    }
                }
            }
        }
    }
}
