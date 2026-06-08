using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Data;
using SnowmobileWPF.Models;
using SnowmobileWPF.Services;

namespace SnowmobileWPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILogger<LoginViewModel> _logger;
        private readonly SecureCredentialService _credentialService;
        private DbSettings _dbSettings;
        private string _serverIp = "(localdb)\\MSSQLLocalDB";
        private string _statusText = "Idle";

        public LoginViewModel(ILogger<LoginViewModel> logger, DbSettings dbSettings, SecureCredentialService credentialService)
        {
            _logger = logger;
            _dbSettings = dbSettings;
            _credentialService = credentialService;

            if (_credentialService.GetServerIp() is string savedIp)
            {
                ServerIp = savedIp;
                _dbSettings.ConnectionString = BuildConnectionString(savedIp);
            }
        }

        public string ServerIp
        {
            get => _serverIp;
            set => SetProperty(ref _serverIp, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        /// <summary>
        /// Attempts to connect using the saved Server IP on startup.
        /// Returns true if the connection succeeds, skipping the login window entirely.
        /// </summary>
        public async Task<bool> TryAutoConnectAsync()
        {
            if (string.IsNullOrEmpty(_dbSettings.ConnectionString))
                return false;

            _logger.LogInformation("Attempting auto-connect to '{ServerIp}'", ServerIp);
            try
            {
                using var connection = new SqlConnection(_dbSettings.ConnectionString);
                await connection.OpenAsync();
                _logger.LogInformation("Auto-connect successful.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Auto-connect failed: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<DbSettings?> Authenticate()
        {
            StatusText = "Connecting...";
            _logger.LogInformation("Connection attempt to Server '{ServerIp}'", ServerIp);

            if (string.IsNullOrWhiteSpace(ServerIp))
            {
                _logger.LogWarning("Connection failed: ServerIp was empty.");
                return null;
            }

            bool success = false;
            int connectionAttempts = 0;
            _dbSettings.ConnectionString = BuildConnectionString(ServerIp);

            while (connectionAttempts < 10)
            {
                connectionAttempts++;
                StatusText = $"Connecting... (try {connectionAttempts})";
                await Task.Delay(200);
                try
                {
                    using var connection = new SqlConnection(_dbSettings.ConnectionString);
                    await connection.OpenAsync();

                    OnConnectSuccess(ref success);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot open database"))
                    {
                        _logger.LogInformation("SnowmobileDb not found on target server — running migrations.");
                        StatusText = "Connected, initializing database...";
                        await ApplyMigrations();
                        OnConnectSuccess(ref success);
                        break;
                    }
                    else
                    {
                        _logger.LogError("Failed to connect to {ServerIp}: {Message}", ServerIp, ex.Message);
                    }
                }
            }

            if (success)
            {
                _logger.LogInformation("Connection successful to '{ServerIp}'", ServerIp);
                return _dbSettings;
            }
            else
            {
                StatusText = "Idle";
                _logger.LogWarning("Connection failed: Could not reach '{ServerIp}'", ServerIp);
                return null;
            }
        }

        private string BuildConnectionString(string serverIp) =>
            new SqlConnectionStringBuilder
            {
                DataSource = serverIp,
                InitialCatalog = "SnowmobileDb",
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                ConnectTimeout = 5
            }.ConnectionString;

        private void OnConnectSuccess(ref bool success)
        {
            StatusText = "Connected!";
            success = true;
            _credentialService.SaveServerIp(ServerIp);
        }

        private async Task ApplyMigrations()
        {
            var options = new DbContextOptionsBuilder<SnowmobileContext>()
                .UseSqlServer(_dbSettings.ConnectionString)
                .Options;

            using var context = new SnowmobileContext(options);
            await context.Database.MigrateAsync();
        }
    }
}