using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Data;
using SnowmobileWPF.Repositories;
using SnowmobileWPF.ViewModels;
using System;
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
                var logger = AppHost.Services.GetRequiredService<ILogger<App>>();
                logger.LogError(ex, "An error occurred while starting the application.");

                MessageBox.Show($"An unexpected error occurred during startup: {ex.Message}",
                                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Shutdown();
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