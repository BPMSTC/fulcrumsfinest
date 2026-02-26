using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using SnowmobileLibrary.Enums;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using System.Globalization;
using System.IO;

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
        public DateTime DateJoined { get; set; } = DateTime.UnixEpoch;

        [Index(10)]
        public bool Active { get; set; } = false;

        //[Index(11)]
        // since ExpDate is 00:00.0 in the entire CSV, let's just set it to default.
        public DateTime ExpDate = DateTime.UnixEpoch;

        [Index(12)]
        public int IssuesLeft { get; set; }

        [Index(13)]
        public string Email { get; set; } = "";

        [Index(14)]
        public bool? Contest { get; set; } = false;

        [Index(15)]
        public DateTime DateRenewed { get; set; } = DateTime.UnixEpoch;

        [Index(16)]
        public bool? ManualMail { get; set; } = false;

        [Index(17)]
        public string Notes { get; set; } = "";

        [Index(18)]
        public bool? Lost { get; set; } = false;

        [Index(19)]
        public string? Source { get; set; }

        [Index(20)]
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
                DateJoined = DateOnly.FromDateTime(this.DateJoined),
                Notes = this.Notes,
                Address = new Address
                {
                    Street = this.Address,
                    City = this.City,
                    Region = this.Region,
                    PostalCode = this.PostalCode,
                    Country = this.Country,
                    VSCA = this.VSCA
                },
                Subscription = new Subscription
                {
                    VSCA = this.VSCA,
                    ExpDate = DateOnly.FromDateTime(this.ExpDate),
                    IssuesRemaining = this.IssuesLeft,
                    DateRenewed = DateOnly.FromDateTime(this.DateRenewed),
                    Source = SourceToEnum(this.Source)
                },
            };
        }
    }
    public class CSVImportService
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public CSVImportService(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public void ImportCSV(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };
            StreamReader reader = new StreamReader(filePath);
            using (CsvReader csv = new CsvReader(reader, config))
            {
                // configure CsvReader to recognize "NULL"
                var options = new TypeConverterOptions { NullValues = { "NULL" } };
                csv.Context.TypeConverterOptionsCache.AddOptions<int>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<int?>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<bool>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<bool?>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

                while (csv.Read())
                {
                    var record = csv.GetRecord<SubscriberCSV>();
                    _subscriberRepository.Create(record.ToSubscriber());
                }
            }
        }
    }
}
