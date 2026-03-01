using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Services;
using SnowmobileWPF.Repositories;
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
                    config.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<SnowmobileContext>(options =>
                        options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

                    // Keep this so you can inject your custom ILogger if needed
                    services.AddSingleton<SnowmobileLibrary.Services.ILogger, FileLogger>();

                    services.AddSingleton<ISubscriberRepository, LocalSubscriberRepository>();
                    services.AddSingleton<MainViewModel>();
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

                var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
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