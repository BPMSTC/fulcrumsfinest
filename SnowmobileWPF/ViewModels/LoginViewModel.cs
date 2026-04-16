using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Data;
using SnowmobileWPF.Models;
using SnowmobileWPF.Services;

namespace SnowmobileWPF.ViewModels
{
    /// <summary>
    /// Handles database authentication and environment setup.
    /// This VM is responsible for validating credentials and ensuring the target SQL server 
    /// is provisioned with the necessary database schema before allowing app entry.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILogger<LoginViewModel> _logger;
        private readonly SecureCredentialService _credentialService;
        private DbSettings _dbSettings;
        private string _serverIp = "(localdb)\\MSSQLLocalDB";
        private string _username = "";
        private string _statusText = "Idle";


        public LoginViewModel(ILogger<LoginViewModel> logger, DbSettings dbSettings, SecureCredentialService credentialService)
        {
            _logger = logger;
            _dbSettings = dbSettings;
            _credentialService = credentialService;

            // Attempts to auto-fill connection details from a secure local cache to streamline the login experience.
            if (_credentialService.GetConnString() is string connString)
            {
                _dbSettings.ConnectionString = connString;
                var builder = new SqlConnectionStringBuilder(_dbSettings.ConnectionString);
                ServerIp = builder.DataSource;
                Username = builder.UserID;
            }
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

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        /// <summary>
        /// Orchestrates the login process with a retry mechanism.
        /// It handles two main scenarios: a standard connection and a scenario where the server 
        /// exists but the specific 'SnowmobileDb' must be created via EF Migrations.
        /// </summary>
        public async Task<DbSettings?> Authenticate(string password)
        {
            StatusText = "Logging in...";
            _logger.LogInformation("Login attempt for user '{Username}' at Server '{ServerIp}'", Username, ServerIp);
            bool success = false;
            int connectionAttempts = 0;
            if (string.IsNullOrWhiteSpace(ServerIp))
            {
                _logger.LogWarning("Login failed: ServerIp was empty.");
                return null;
            }

            // Polling/Retry loop to account for slow SQL instance wake-ups or network latency
            while (connectionAttempts < 10)
            {
                connectionAttempts++;
                StatusText = $"Logging in... (try {connectionAttempts})";
                await Task.Delay(200);
                try
                {
                    // creates connection string, needed for connections to the database
                    var builder = new SqlConnectionStringBuilder
                    {
                        DataSource = ServerIp,
                        UserID = Username,
                        Password = password,
                        InitialCatalog = "SnowmobileDb",
                        TrustServerCertificate = true,
                        ConnectTimeout = 5
                    };
                    _dbSettings.ConnectionString = builder.ConnectionString;

                    // test connection
                    using var connection = new SqlConnection(_dbSettings.ConnectionString);
                    await connection.OpenAsync();

                    // this can only run if the connection is successfully opened
                    OnAuthSuccess(ref success);
                    break;
                }
                catch (Exception ex)
                {
                    // Catching "Cannot open database" specifically allows for "Just-in-Time" database provisioning
                    if (ex.Message.Contains("Cannot open database"))
                    {
                        _logger.LogInformation("Target SQL Server does not have SnowmobileDb.");
                        StatusText = "Login successful, initializing database...";
                        await ApplyMigrations();
                        OnAuthSuccess(ref success);
                        break;
                    }
                    else
                    {
                        _logger.LogError($"Failed to connect to {ServerIp}: {ex.Message}");
                    }
                }
            }

            if (success)
            {
                _logger.LogInformation("Login successful for '{Username}'", Username);
                return _dbSettings;
            }
            else
            {
                StatusText = "Idle";
                _logger.LogWarning("Login failed: Invalid credentials for '{Username}'", Username);
                return null;
            }
        }

        private void OnAuthSuccess(ref bool success)
        {
            StatusText = "Login complete!";
            success = true;
            _credentialService.SaveConnectionString(_dbSettings);
        }

        /// <summary>
        /// Leverages EF Core to push the latest schema to a newly discovered or outdated SQL instance.
        /// Ensures the application can self-heal or deploy its own database structure upon first login.
        /// </summary>
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