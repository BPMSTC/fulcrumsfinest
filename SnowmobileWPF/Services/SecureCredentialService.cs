using Microsoft.Extensions.Logging;
using System.IO;

namespace SnowmobileWPF.Services
{
    public class SecureCredentialService
    {
        private readonly ILogger<SecureCredentialService> _logger;
        private readonly string _settingsFileLocation;

        public SecureCredentialService(ILogger<SecureCredentialService> logger)
        {
            _settingsFileLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VSCASubscriberManager", "settings.dat");
            _logger = logger;
        }

        public void SaveServerIp(string serverIp)
        {
            try
            {
                _logger.LogInformation("Saving server IP...");
                var directory = Path.GetDirectoryName(_settingsFileLocation)!;
                Directory.CreateDirectory(directory);
                File.WriteAllText(_settingsFileLocation, serverIp);
                _logger.LogInformation("Server IP saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving server IP: {Message}", ex.Message);
            }
        }

        public string? GetServerIp()
        {
            try
            {
                if (!File.Exists(_settingsFileLocation))
                {
                    _logger.LogWarning("Settings file not found at {Location}", _settingsFileLocation);
                    return null;
                }
                var serverIp = File.ReadAllText(_settingsFileLocation).Trim();

                // If the file contains null bytes it's the legacy DPAPI-encrypted format — discard it.
                if (serverIp.Contains('\0'))
                {
                    _logger.LogWarning("Settings file contains legacy encrypted data. Discarding.");
                    File.Delete(_settingsFileLocation);
                    return null;
                }

                _logger.LogInformation("Server IP retrieved successfully.");
                return string.IsNullOrEmpty(serverIp) ? null : serverIp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server IP: {Message}", ex.Message);
                return null;
            }
        }
    }
}