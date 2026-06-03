using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText.Text = version != null ? $"v{version.Major}.{version.Minor}.{version.Build}" : "v1.0.0";
        }

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