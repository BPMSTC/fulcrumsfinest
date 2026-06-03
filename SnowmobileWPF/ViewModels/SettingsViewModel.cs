using Microsoft.Extensions.Logging;
using SnowmobileWPF.Repositories;
using SnowmobileWPF.Services;
using System.Windows;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISubscriberRepository _repository;
        private readonly SubscriptionExpirationService _expirationService;
        private readonly ILogger<SettingsViewModel> _logger;

        // Set by SettingsWindow code-behind to close itself before logout runs.
        public Action? CloseWindow { get; set; }

        // Invoked after actions that modify subscriber data so the main window refreshes.
        public Action? OnDataChanged { get; set; }

        // Invoked when the user confirms logout.
        public Action? OnLogoutRequested { get; set; }

        public ICommand ClearAllSubscribersCommand { get; }
        public ICommand ForceExpirationCheckCommand { get; }
        public ICommand LogoutCommand { get; }

        public SettingsViewModel(
            ISubscriberRepository repository,
            SubscriptionExpirationService expirationService,
            ILogger<SettingsViewModel> logger)
        {
            _repository = repository;
            _expirationService = expirationService;
            _logger = logger;

            ClearAllSubscribersCommand = new RelayCommand(_ => ExecuteClearAllSubscribers());
            ForceExpirationCheckCommand = new RelayCommand(_ => ExecuteForceExpirationCheck());
            LogoutCommand = new RelayCommand(_ => ExecuteLogout());
        }

        private void ExecuteClearAllSubscribers()
        {
            var first = MessageBox.Show(
                "This will permanently delete ALL subscribers and their data. This cannot be undone.\n\nAre you sure?",
                "Clear All Subscribers",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (first != MessageBoxResult.Yes) return;

            var second = MessageBox.Show(
                "Final warning: ALL subscriber data will be permanently deleted. There is no recovery.\n\nContinue?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);

            if (second != MessageBoxResult.Yes) return;

            _repository.DeleteAll();
            _logger.LogWarning("All subscribers deleted via Settings.");
            OnDataChanged?.Invoke();
            MessageBox.Show("All subscribers have been deleted.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void ExecuteForceExpirationCheck()
        {
            await _expirationService.DeactivateExpiredAsync();
            _logger.LogInformation("Manual expiration check completed via Settings.");
            OnDataChanged?.Invoke();
            MessageBox.Show(
                "Expiration check complete. Any subscribers with expired subscriptions have been deactivated.",
                "Done", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteLogout()
        {
            var result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Log Out",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CloseWindow?.Invoke();
                OnLogoutRequested?.Invoke();
            }
        }
    }
}
