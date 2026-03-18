using Microsoft.Extensions.Logging;
using SnowmobileWPF.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SnowmobileWPF.Services
{
    public class SecureCredentialService
    {
        private readonly ILogger<SecureCredentialService> _logger;
        private readonly string _credentialFileLocation;
        public SecureCredentialService(ILogger<SecureCredentialService> logger) 
        {
            _credentialFileLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VSCASubscriberManager", "settings.dat");
            _logger = logger;
        }

        public void SaveConnectionString(DbSettings dbSettings)
        {
            try
            {
                _logger.LogInformation("Preparing to save connection string...");
                byte[] connString = UnicodeEncoding.UTF8.GetBytes(dbSettings.ConnectionString);
                byte[] encryptedData = ProtectedData.Protect(connString, null, DataProtectionScope.CurrentUser);

                File.WriteAllBytes(_credentialFileLocation, encryptedData);
                _logger.LogInformation("Connection string saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving connection string: {Message}", ex.Message);
            }
        }

        public string? GetConnString()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve connection string...");
                if (!File.Exists(_credentialFileLocation))
                {
                    _logger.LogWarning("Credential file not found at {Location}", _credentialFileLocation);
                    return null;
                }
                byte[] encryptedData = File.ReadAllBytes(_credentialFileLocation);
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
                string connString = UnicodeEncoding.UTF8.GetString(decryptedData);
                _logger.LogInformation("Connection string retrieved successfully.");
                return connString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving connection string: {Message}", ex.Message);
                return null;
            }
        }
    }
}
