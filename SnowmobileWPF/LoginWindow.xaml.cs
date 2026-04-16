using System.Windows;
using SnowmobileWPF.Models;
using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF
{
    public partial class LoginWindow : Window
    {
        public DbSettings? Settings { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
            Settings = null;
        }

        /// <summary>
        /// Handles the secure password extraction. 
        /// PasswordBox is not dependency-property bound for security reasons, 
        /// so we manually pass the clear-text password to the ViewModel for authentication.
        /// </summary>
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                string password = UserPasswordBox.Password;
                Settings = await vm.Authenticate(password);
                if (Settings != null)
                {
                    DialogResult = true;
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