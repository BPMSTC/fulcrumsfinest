using System.Reflection;
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
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText.Text = version != null ? $"v{version.Major}.{version.Minor}.{version.Build}" : "v1.0.0";
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                Settings = await vm.Authenticate();
                if (Settings != null)
                {
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Could not connect to the server. Check the Server IP and ensure your Windows account has access.",
                                    "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}