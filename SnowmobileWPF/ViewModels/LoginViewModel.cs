using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private string _username = "";
        private string _statusText = "Idle";


        public LoginViewModel(ILogger<LoginViewModel> logger, DbSettings dbSettings, SecureCredentialService credentialService)
        {
            _logger = logger;
            _dbSettings = dbSettings;
            _credentialService = credentialService;
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
                } catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot open database"))
                    {
                        _logger.LogInformation("Target SQL Server does not have SnowmobileDb.");
                        StatusText = "Login successful, initializing database...";
                        await ApplyMigrations();
                        OnAuthSuccess(ref success);
                        break;
                    } else
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