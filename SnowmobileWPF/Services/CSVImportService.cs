using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Enums;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using SnowmobileWPF.ViewModels;
using System.Globalization;
using System.IO;
using System.Windows;

namespace SnowmobileWPF.Services
{
    class SubscriberCSV
    {
        [Index(0)]
        public int VSCA { get; set; }

        [Index(1)]
        public string LastName { get; set; }

        [Index(2)]
        public string FirstName { get; set; }

        [Index(3)]
        public string Address { get; set; }

        [Index(4)]
        public string City { get; set; }

        [Index(5)]
        public string Region { get; set; }

        [Index(6)]
        public string PostalCode { get; set; }

        [Index(7)]
        public string Country { get; set; }

        [Index(8)]
        public string Phone { get; set; } = "";

        [Index(9)]
        public DateOnly? DateJoined { get; set; }

        [Index(10)]
        public bool Active { get; set; } = false;

        [Index(11)]
        // since ExpDate is 00:00.0 in the entire CSV, let's just set it to default.
        public DateTime? ExpDate { get; set; }

        [Index(12)]
        public int IssuesLeft { get; set; }

        [Index(13)]
        public string Email { get; set; } = "";

        [Index(14)]
        public bool? Contest { get; set; } = false;

        [Index(15)]
        public DateOnly? DateRenewed { get; set; } = DateOnly.FromDateTime(DateTime.UnixEpoch);

        [Index(16)]
        public bool? ManualMail { get; set; } = false;

        [Index(17)]
        public string Notes { get; set; } = "";

        [Index(18)]
        public string? Source { get; set; }

        [Index(19)]
        public bool? Commercial { get; set; } = false;

        public SubscriptionSource? SourceToEnum(string source)
        {
            switch (source)
            {
                case "Mail":
                    return SubscriptionSource.PostalMail;
                case "Person":
                    return SubscriptionSource.InPerson;
                case "Phone":
                    return SubscriptionSource.Phone;
                case "Web":
                    return SubscriptionSource.Internet;
                default:
                    return null;
            }
        }

        // some customer records have multiple emails in a field. we'll take the first email and disregard the rest.
        private string HandleEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return string.Empty;
            }

            // Split by common email separators (comma, semicolon, space) and return the first email
            var emails = email.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            return emails.Length > 0 ? emails[0] : string.Empty;
        }

        public Subscriber ToSubscriber()
        {
            return new Subscriber
            {
                VSCA = this.VSCA,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Phone = this.Phone,
                Active = this.Active,
                Contest = this.Contest ?? false,
                ManualMail = this.ManualMail ?? false,
                Commercial = this.Commercial ?? false,
                DateJoined = this.DateJoined ?? DateOnly.FromDateTime(DateTime.UnixEpoch),
                Notes = this.Notes,
                Email = HandleEmail(this.Email),
                Address = new Address
                {
                    Street = this.Address,
                    City = this.City,
                    Region = this.Region,
                    PostalCode = this.PostalCode,
                    Country = this.Country
                },
                Subscription = new Subscription
                {
                    ExpDate = DateOnly.FromDateTime(this.ExpDate ?? DateTime.UnixEpoch),
                    IssuesRemaining = this.IssuesLeft,
                    DateRenewed = this.DateRenewed ?? DateOnly.FromDateTime(DateTime.UnixEpoch),
                    Source = SourceToEnum(this.Source)
                },
            };
        }
    }
    public class CSVImportService
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILogger<CSVImportService> _logger;

        public CSVImportService(ISubscriberRepository subscriberRepository, ILogger<CSVImportService> logger)
        {
            _subscriberRepository = subscriberRepository;
            _logger = logger;
        }

        public async Task ImportCSV(string filePath, IProgress<int> progress)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            StreamReader reader;

            // attempt to open the file, and if it fails, show an error message and exit the method
            try
            {
                reader = new StreamReader(filePath);
            } catch (Exception e)
            {
                MessageBox.Show("Error opening file: " + e.Message, "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (CsvReader csv = new CsvReader(reader, config))
            {
                // configure CsvReader to recognize "NULL" and other null placeholders
                var options = new TypeConverterOptions { NullValues = { "NULL", String.Empty, "0:00.0" } };
                csv.Context.TypeConverterOptionsCache.AddOptions<int>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<int?>(options);
                //csv.Context.TypeConverterOptionsCache.AddOptions<string>(options);
                //csv.Context.TypeConverterOptionsCache.AddOptions<string?>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<bool>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<bool?>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateOnly>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateOnly?>(options);

                // since we're carrying over old VSCA numbers, we tell SQL Server to allow explicit values for VSCA
                var tx = _subscriberRepository.StartTx();
                _subscriberRepository.SetIdentityInsert(true);
                while (csv.Read())
                {
                    var record = csv.GetRecord<SubscriberCSV>();
                    await Task.Run(() =>
                    {
                        try
                        {
                            _subscriberRepository.Create(record.ToSubscriber(), true);
                        }
                        catch (Exception ex)
                        {
                            // Log the error and continue with the next record
                            _logger.LogError($"Failed row {record.VSCA}");
                            _logger.LogError($"Error importing record with VSCA {record.VSCA}: {ex.ToString()}");
                        }
                    });

                    // report import progress as a percentage
                    progress?.Report((int)((double)reader.BaseStream.Position / reader.BaseStream.Length * 100));
                }
                _subscriberRepository.SetIdentityInsert(false);
                tx.Commit();
            }
        }
    }
}
