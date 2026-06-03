using Microsoft.Extensions.DependencyInjection;
using SnowmobileLibrary.Enums;
using SnowmobileWPF.Services;
using SnowmobileWPF.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnowmobileWPF
{
    public partial class MainWindow : Window
    {
        private SearchWindow searchWindow;
        public MainWindow()
        {
            InitializeComponent();
            SourceComboBox.ItemsSource = Enum.GetValues(typeof(SubscriptionSource));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // checks if searchWindow is already opened
            if (searchWindow == null || !searchWindow.IsLoaded)
            {
                searchWindow = new SearchWindow
                {
                    Owner = this
                };
                searchWindow.Closed += (s, e) =>
                {
                    if (DataContext is MainViewModel vm)
                        vm.LoadAndRestoreSelection();
                };
            }
            if (searchWindow.WindowState == WindowState.Minimized)
            {
                searchWindow.WindowState = WindowState.Normal;
            }
            searchWindow.Show();
        }

        private void SubscriberList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedSubscriber != null)
            {
                vm.UpdateCommand.Execute(null);
            }
        }

        private void EditNotesButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (NotesTextBox != null)
                {
                    NotesTextBox.Focus();
                    NotesTextBox.CaretIndex = NotesTextBox.Text.Length;
                }
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.LogoutAction = async () => await HandleLogoutAsync();
        }

        private async Task HandleLogoutAsync()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.Hide();

            // Reset login status text so the re-login screen looks fresh
            var loginVm = App.AppHost.Services.GetRequiredService<LoginViewModel>();
            loginVm.StatusText = "Idle";

            var loginWindow = App.AppHost.Services.GetRequiredService<LoginWindow>();
            if (loginWindow.ShowDialog() == true)
            {
                var expirationService = App.AppHost.Services.GetRequiredService<SubscriptionExpirationService>();
                await expirationService.DeactivateExpiredAsync();

                if (DataContext is MainViewModel vm)
                {
                    vm.SelectedSubscriber = null;
                    vm.LoadSubscribers();
                }

                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                this.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void ContestButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.CheckAcknowledged)
                {
                    ToolTip contestToolTip = new ToolTip
                    {
                        Content = "A contest has ended since last login.",
                        Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                        PlacementTarget = ContestButton,
                        StaysOpen = false
                    };
                    contestToolTip.IsOpen = true;
                }
            }
        }
    }
}