using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var logger = App.AppHost.Services.GetRequiredService<ILogger<LoginViewModel>>();
            DataContext = new LoginViewModel(logger);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                string password = UserPasswordBox.Password;

                if (vm.Authenticate(password))
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