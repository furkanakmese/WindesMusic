using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            lblMessage.Text = "";
            var name = inputName.Text;
            var email = inputEmail.Text;
            var password = inputPassword.Password;
            var passwordRepeat = inputPasswordRepeat.Password;

            if (password == passwordRepeat)
            {
                var random = new RNGCryptoServiceProvider();
                int max_length = 32;
                byte[] salt = new byte[max_length];
                random.GetNonZeroBytes(salt);
                string saltText = Encoding.ASCII.GetString(salt);
                

                Database db = new Database();
                if (db.Register(name, email, password, saltText).Email != null)
                {
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                } else
                {
                    lblMessage.Text = "User with that email already registered";
                }
            } else
            {
                lblMessage.Text = "Passwords must be the same";
            }
        }
    }
}
