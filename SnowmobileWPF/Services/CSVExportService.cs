using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Enums;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using System.Globalization;
using System.IO;
using System.Windows;

namespace SnowmobileWPF.Services
{
    // Mirrors the column layout of SubscriberCSV used during import so files are round-trippable.
    class SubscriberCSVExport
    {
        [Index(0)]  public int VSCA { get; set; }
        [Index(1)]  public string LastName { get; set; } = "";
        [Index(2)]  public string FirstName { get; set; } = "";
        [Index(3)]  public string Address { get; set; } = "";
        [Index(4)]  public string City { get; set; } = "";
        [Index(5)]  public string Region { get; set; } = "";
        [Index(6)]  public string PostalCode { get; set; } = "";
        [Index(7)]  public string Country { get; set; } = "";
        [Index(8)]  public string Phone { get; set; } = "";
        [Index(9)]  public DateOnly? DateJoined { get; set; }
        [Index(10)] public bool Active { get; set; }
        [Index(11)] public DateTime? ExpDate { get; set; }
        [Index(12)] public int IssuesLeft { get; set; }
        [Index(13)] public string Email { get; set; } = "";
        [Index(14)] public bool Contest { get; set; }
        [Index(15)] public DateOnly? DateRenewed { get; set; }
        [Index(16)] public bool ManualMail { get; set; }
        [Index(17)] public string Notes { get; set; } = "";
        [Index(18)] public string? Source { get; set; }
        [Index(19)] public bool Commercial { get; set; }

        public static SubscriberCSVExport FromSubscriber(Subscriber s) => new SubscriberCSVExport
        {
            VSCA        = s.VSCA,
            LastName    = s.LastName,
            FirstName   = s.FirstName,
            Address     = s.Address?.Street ?? "",
            City        = s.Address?.City ?? "",
            Region      = s.Address?.Region ?? "",
            PostalCode  = s.Address?.PostalCode ?? "",
            Country     = s.Address?.Country ?? "",
            Phone       = s.Phone ?? "",
            DateJoined  = s.DateJoined,
            Active      = s.Active,
            ExpDate     = s.Subscription?.ExpDate.ToDateTime(new TimeOnly(0)),
            IssuesLeft  = s.Subscription?.IssuesRemaining ?? 0,
            Email       = s.Email ?? "",
            Contest     = s.Contest,
            DateRenewed = s.Subscription?.DateRenewed,
            ManualMail  = s.ManualMail,
            Notes       = s.Notes ?? "",
            Source      = EnumToSource(s.Subscription?.Source),
            Commercial  = s.Commercial
        };

        private static string? EnumToSource(SubscriptionSource? source) => source switch
        {
            SubscriptionSource.PostalMail => "Mail",
            SubscriptionSource.InPerson   => "Person",
            SubscriptionSource.Phone      => "Phone",
            SubscriptionSource.Internet   => "Web",
            _                             => null
        };
    }

    public class CSVExportService
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILogger<CSVExportService> _logger;

        public CSVExportService(ISubscriberRepository subscriberRepository, ILogger<CSVExportService> logger)
        {
            _subscriberRepository = subscriberRepository;
            _logger = logger;
        }

        public async Task ExportCSV(string filePath, IProgress<int> progress)
        {
            try
            {
                var subscribers = _subscriberRepository.Retrieve(-1);
                int total = subscribers.Count;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };

                await using var writer = new StreamWriter(filePath);
                await using var csv = new CsvWriter(writer, config);

                for (int i = 0; i < total; i++)
                {
                    csv.WriteRecord(SubscriberCSVExport.FromSubscriber(subscribers[i]));
                    await csv.NextRecordAsync();

                    if (total > 0)
                        progress?.Report((int)((double)(i + 1) / total * 100));
                }

                _logger.LogInformation($"Export complete. {total} record(s) written to {filePath}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Export failed: {ex.Message}");
                MessageBox.Show("Error exporting file: " + ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
