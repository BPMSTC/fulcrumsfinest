using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Services;
using SnowmobileWPF.Models;

namespace SnowmobileWPF.Services
{
    // Background service that periodically deactivates subscribers whose subscriptions have expired.
    public class SubscriptionExpirationService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly DbSettings _dbSettings;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public SubscriptionExpirationService(ILogger logger, DbSettings dbSettings)
        {
            _logger = logger;
            _dbSettings = dbSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("SubscriptionExpirationService started.");

            // Wait for the user to log in before doing any work.
            while (string.IsNullOrEmpty(_dbSettings.ConnectionString))
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

            using var timer = new PeriodicTimer(_interval);

            // Periodic runs only — the startup run is handled explicitly in App.xaml.cs
            // so it completes before the main window loads subscribers.
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DeactivateExpiredSubscribersAsync(stoppingToken);
            }
        }

        // Called directly from App.xaml.cs after login, before the main window is shown.
        public Task DeactivateExpiredAsync() => DeactivateExpiredSubscribersAsync(CancellationToken.None);

        private async Task DeactivateExpiredSubscribersAsync(CancellationToken stoppingToken)
        {
            try
            {
                var options = new DbContextOptionsBuilder<SnowmobileContext>()
                    .UseSqlServer(_dbSettings.ConnectionString)
                    .Options;

                await using var context = new SnowmobileContext(options);

                var today = DateOnly.FromDateTime(DateTime.Today);

                var expiredSubscribers = await context.Subscribers
                    .Include(s => s.Subscription)
                    .Where(s => s.Active && s.Subscription.ExpDate < today)
                    .ToListAsync(stoppingToken);

                if (expiredSubscribers.Count == 0)
                    return;

                foreach (var subscriber in expiredSubscribers)
                    subscriber.Active = false;

                await context.SaveChangesAsync(stoppingToken);

                _logger.LogInfo($"SubscriptionExpirationService: deactivated {expiredSubscribers.Count} expired subscriber(s).");
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown — no need to log.
            }
            catch (Exception ex)
            {
                _logger.LogError("SubscriptionExpirationService encountered an error while deactivating expired subscribers.", ex);
            }
        }
    }
}