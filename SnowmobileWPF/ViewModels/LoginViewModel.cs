using Microsoft.Extensions.Logging;

namespace SnowmobileWPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILogger<LoginViewModel> _logger;
        private string _serverIp = "127.0.0.1:1433";
        private string _username = string.Empty;

        public LoginViewModel(ILogger<LoginViewModel> logger)
        {
            _logger = logger;
        }

        public string ServerIp
        {
            get => _serverIp;
            set => SetProperty(ref _serverIp, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public bool Authenticate(string password)
        {
            _logger.LogInformation("Login attempt for user '{Username}' at Server '{ServerIp}'", Username, ServerIp);

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Login failed: Username or Password was empty.");
                return false;
            }

            // Placeholder for real logic
            bool success = true;

            if (success)
                _logger.LogInformation("Login successful for '{Username}'", Username);
            else
                _logger.LogWarning("Login failed: Invalid credentials for '{Username}'", Username);

            return success;
        }
    }
}