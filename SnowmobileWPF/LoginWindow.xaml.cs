using System.Windows;
using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                // Pull the password safely from the PasswordBox
                string password = UserPasswordBox.Password;

                if (vm.Authenticate(password))
                {
                    DialogResult = true; // Signals successful login
                }
                else
                {
                    MessageBox.Show("Invalid credentials or Server IP.", "Login Failed",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}