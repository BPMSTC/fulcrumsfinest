using SnowmobileLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnowmobileWPF.Repositories
{
    public class LocalSubscriberRepository : ISubscriberRepository
    {
        private readonly List<Subscriber> subscribers = new();

        public LocalSubscriberRepository()
        {
            Subscriber subscriber = new Subscriber
            {
                VSCA = 12345,
                FirstName = "John",
                LastName = "Doe",
                Phone = "715-867-5309",
                Active = true,
                Contest = false,
                ManualMail = false,
                Commercial = false,
                DateJoined = new DateOnly(2020, 1, 1),
                Address = new Address
                {
                    AddressId = new Random().Next(1, 100000),
                    Street = "123 Main St",
                    City = "Anytown",
                    Region = "WI",
                    PostalCode = "12345",
                    Country = "USA",
                    IsActive = true
                },
                Subscription = new Subscription
                {
                    SubscriptionId = new Random().Next(1, 100000),
                    DateRenewed = new DateOnly(2020, 1, 1),
                    ExpDate = new DateOnly(2021, 1, 1),
                    Source = SnowmobileLibrary.Enums.SubscriptionSource.Internet
                },
                Email = new Email
                {
                    EmailAddress = "jdoe@example.com"
                }
            };

            subscribers.Add(subscriber);
        }

        public List<Subscriber>? Search(SearchParams searchParams)
        {
            IEnumerable<Subscriber> results = subscribers;

            if (searchParams.VSCA != null)
            {
                results = results.Where(s => s.VSCA == searchParams.VSCA);
            }

            if (!string.IsNullOrEmpty(searchParams.FirstName))
            {
                results = results.Where(s =>
                    s.FirstName.Contains(searchParams.FirstName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchParams.LastName))
            {
                results = results.Where(s =>
                    s.LastName.Contains(searchParams.LastName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchParams.PhoneNumber))
            {
                results = results.Where(s =>
                    s.Phone.Contains(searchParams.PhoneNumber));
            }

            return results
                .OrderByDescending(s => s.VSCA)
                .ToList();
        }

        public List<Subscriber> Retrieve(int max)
        {
            IEnumerable<Subscriber> results = subscribers
                .OrderByDescending(s => s.VSCA);

            if (max > 0)
            {
                results = results.Take(max);
            }

            return results.ToList();
        }

        public void Create(Subscriber subscriber, bool forceCreation = false)
        {
            SearchParams searchParams = new()
            {
                FirstName = subscriber.FirstName,
                LastName = subscriber.LastName
            };

            // Prevent duplicates unless forced
            if (Search(searchParams)?.Count > 0 && !forceCreation)
            {
                throw new Exception("Subscriber already exists");
            }

            subscribers.Add(subscriber);
        }

        public void Update(Subscriber subscriber)
        {
            Delete(subscriber);
            Create(subscriber, true);
        }

        public void Delete(Subscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }
    }
}