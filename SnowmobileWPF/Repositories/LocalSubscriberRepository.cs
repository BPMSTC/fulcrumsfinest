using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Models;

namespace SnowmobileWPF.Repositories
{
    public class LocalSubscriberRepository : ISubscriberRepository
    {
        private readonly List<Subscriber> subscribers = new();
        private readonly ILogger<LocalSubscriberRepository> _logger;

        public LocalSubscriberRepository(ILogger<LocalSubscriberRepository> logger)
        {
            _logger = logger;
            _logger.LogInformation("Initializing LocalSubscriberRepository with sample data.");

            // Initial Seed Data
            subscribers.Add(new Subscriber
            {
                VSCA = 12345,
                FirstName = "John",
                LastName = "Doe",
                Phone = "715-867-5309",
                Active = true,
                DateJoined = new DateOnly(2020, 1, 1),
                Email = "jdoe@example.com",
                Address = new Address
                {
                    AddressId = 1,
                    Street = "123 Main St",
                    City = "Anytown",
                    Region = "WI",
                    PostalCode = "12345",
                    Country = "USA",
                    IsActive = true
                },
                Subscription = new Subscription
                {
                    SubscriptionId = 1,
                    DateRenewed = new DateOnly(2020, 1, 1),
                    ExpDate = new DateOnly(2021, 1, 1),
                    Source = SnowmobileLibrary.Enums.SubscriptionSource.Internet
                }
            });
        }

        public List<Subscriber>? Search(SearchParams searchParams)
        {
            _logger.LogInformation("Executing Search with parameters: VSCA={VSCA}, First={FirstName}, Last={LastName}",
                searchParams.VSCA, searchParams.FirstName, searchParams.LastName);

            IEnumerable<Subscriber> results = subscribers;

            if (searchParams.VSCA != null)
                results = results.Where(s => s.VSCA == searchParams.VSCA);

            if (!string.IsNullOrEmpty(searchParams.FirstName))
                results = results.Where(s => s.FirstName.Contains(searchParams.FirstName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(searchParams.LastName))
                results = results.Where(s => s.LastName.Contains(searchParams.LastName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(searchParams.PhoneNumber))
                results = results.Where(s => s.Phone.Contains(searchParams.PhoneNumber));

            var finalResults = results.OrderByDescending(s => s.VSCA).ToList();
            _logger.LogDebug("Search returned {Count} results.", finalResults.Count);

            return finalResults;
        }

        public List<Subscriber> Retrieve(int max)
        {
            _logger.LogInformation("Retrieving subscribers (Max: {Max})", max);

            IEnumerable<Subscriber> results = subscribers.OrderByDescending(s => s.VSCA);

            if (max > 0)
                results = results.Take(max);

            return results.ToList();
        }

        public void Create(Subscriber subscriber, bool forceCreation = false)
        {
            _logger.LogInformation("Attempting to create subscriber: {FirstName} {LastName}", subscriber.FirstName, subscriber.LastName);

            SearchParams searchParams = new()
            {
                FirstName = subscriber.FirstName,
                LastName = subscriber.LastName
            };

            if (Search(searchParams)?.Count > 0 && !forceCreation)
            {
                _logger.LogWarning("Create failed: Subscriber {FirstName} {LastName} already exists.", subscriber.FirstName, subscriber.LastName);
                throw new ArgumentException("Subscriber already exists");
            }

            subscribers.Add(subscriber);
            _logger.LogInformation("Successfully created subscriber VSCA: {VSCA}", subscriber.VSCA);
        }

        public void Update(Subscriber subscriber)
        {
            _logger.LogInformation("Updating subscriber VSCA: {VSCA}", subscriber.VSCA);

            Delete(subscriber);
            Create(subscriber, true);
        }

        public void Delete(Subscriber subscriber)
        {
            _logger.LogWarning("Deleting subscriber VSCA: {VSCA} ({FirstName} {LastName})",
                subscriber.VSCA, subscriber.FirstName, subscriber.LastName);

            subscribers.Remove(subscriber);
        }

        public void DeleteAll()
        {
            _logger.LogWarning("Deleting all subscribers.");
            subscribers.Clear();
        }

        public void SetIdentityInsert(bool enabled)
        {
            throw new NotImplementedException();
        }

        public IDbContextTransaction StartTx()
        {
            throw new NotImplementedException();
        }
    }
}