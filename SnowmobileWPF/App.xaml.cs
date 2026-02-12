using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Data;
using System.Windows;

namespace SnowmobileWPF
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; } = null!;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Load appsettings.json for local DB connection
                    config.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((context, services) =>
                {
                    // Get connection string from configuration
                    var connectionString = context.Configuration
                        .GetConnectionString("DefaultConnection");

                    services.AddDbContext<SnowmobileContext>(options =>
                        options.UseSqlServer(connectionString));

                    services.AddTransient<MainWindow>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(); // For development; can add file or other providers later
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppHost.StartAsync();

                var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Log exception and show user-friendly message
                var logger = AppHost.Services.GetRequiredService<ILogger<App>>();
                logger.LogError(ex, "An error occurred while starting the application.");

                MessageBox.Show("An unexpected error occurred during startup. Please see the logs.",
                                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Shutdown(); // Stop the application if startup fails
            }
            finally
            {
                base.OnStartup(e);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                await AppHost.StopAsync();
            }
            catch (Exception ex)
            {
                var logger = AppHost.Services.GetRequiredService<ILogger<App>>();
                logger.LogError(ex, "An error occurred while stopping the application.");
            }
            finally
            {
                AppHost.Dispose();
                base.OnExit(e);
            }
        }
    }
}