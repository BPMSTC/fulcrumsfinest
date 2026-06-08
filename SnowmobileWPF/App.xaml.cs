using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Services;
using SnowmobileWPF.Models;
using SnowmobileWPF.Repositories;
using SnowmobileWPF.Services;
using SnowmobileWPF.ViewModels;
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
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<DbSettings>();
                    services.AddSingleton<LoginViewModel>();
                    services.AddTransient<LoginWindow>(s => new LoginWindow
                    {
                        DataContext = s.GetRequiredService<LoginViewModel>()
                    });

                    services.AddDbContextFactory<SnowmobileContext>((sp, options) =>
                    {
                        var settings = sp.GetRequiredService<DbSettings>();
                        options.UseSqlServer(settings.ConnectionString);
                    });
                    

                    services.AddSingleton<SnowmobileLibrary.Services.ILogger, FileLogger>();

                    services.AddSingleton<SubscriptionExpirationService>();
                    services.AddHostedService(s => s.GetRequiredService<SubscriptionExpirationService>());
                    services.AddSingleton<ISubscriberRepository, SubscriberRepository>();
                    services.AddSingleton<IContestRepository, ContestRepository>();
                    services.AddSingleton<SecureCredentialService>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<ContestViewModel>();
                    services.AddSingleton<RenewViewModel>();
                    services.AddSingleton<SettingsViewModel>();
                    services.AddSingleton<MainWindow>(s => new MainWindow
                    {
                        DataContext = s.GetRequiredService<MainViewModel>()
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();

                    logging.AddProvider(new FileLoggerProvider());

                    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // 1. Setup Global Exception Handling
            var customLogger = AppHost.Services.GetRequiredService<SnowmobileLibrary.Services.ILogger>();

            this.DispatcherUnhandledException += (s, args) =>
            {
                customLogger.LogError("FATAL UNHANDLED UI EXCEPTION", args.Exception);
                MessageBox.Show("A fatal error occurred. The application will close.", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
                Shutdown();
            };

            try
            {
                customLogger.LogInfo("Application Startup Sequence Initiated.");
                await AppHost.StartAsync();
                
                // set ShutdownMode to prevent app closing when login window is closed
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // Attempt auto-connect using saved Server IP (Windows Authentication).
                // On success the login window is skipped entirely.
                var loginVm = AppHost.Services.GetRequiredService<LoginViewModel>();
                bool autoConnected = await loginVm.TryAutoConnectAsync();

                if (!autoConnected)
                {
                    var loginWindow = AppHost.Services.GetRequiredService<LoginWindow>();
                    if (loginWindow.ShowDialog() != true)
                    {
                        Shutdown();
                        return;
                    }
                }

                // Run expiration check before the main window loads subscribers,
                // so the UI reflects up-to-date Active values from the start.
                var expirationService = AppHost.Services.GetRequiredService<SubscriptionExpirationService>();
                await expirationService.DeactivateExpiredAsync();

                var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();

                // revert ShutdownMode now that the main window is open
                this.MainWindow = mainWindow;
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;

                mainWindow.Show();
            }
            catch (Exception ex)
            {
                customLogger.LogError("Application failed to start.", ex);
                MessageBox.Show($"Startup Error: {ex.Message}");
                Shutdown();
            }
            finally
            {
                base.OnStartup(e);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            var customLogger = AppHost.Services.GetRequiredService<SnowmobileLibrary.Services.ILogger>();
            try
            {
                customLogger.LogInfo("Application Shutting Down.");
                await AppHost.StopAsync();
            }
            catch (Exception ex)
            {
                customLogger.LogError("Error during shutdown.", ex);
            }
            finally
            {
                AppHost.Dispose();
                base.OnExit(e);
            }
        }
    }
}