using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Models;

namespace SnowmobileWPF.Repositories
{
    // Handles database operations for Subscriber entities.
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly SnowmobileContext _context;

        public SubscriberRepository(IDbContextFactory<SnowmobileContext> factory)
        {
            _context = factory.CreateDbContext();
        }

        public List<Subscriber>? Search(SearchParams searchParams)
        {
            // Base query with eager loading so Address and Subscription
            // are retrieved with the Subscriber in one database call.
            var query = _context.Subscribers
                .Include(s => s.Address)
                .Include(s => s.Subscription)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchParams.LastName))
                query = query.Where(s => s.LastName.Contains(searchParams.LastName));

            if (!string.IsNullOrEmpty(searchParams.FirstName))
                query = query.Where(s => s.FirstName.Contains(searchParams.FirstName));

            if (!string.IsNullOrEmpty(searchParams.PhoneNumber))
                query = query.Where(s => s.Phone.Contains(searchParams.PhoneNumber));

            if (searchParams.VSCA.HasValue)
                query = query.Where(s => s.VSCA == searchParams.VSCA.Value);

            // Execute query
            return query
                .OrderByDescending(s => s.VSCA)
                .ToList();
        }

        public List<Subscriber> Retrieve(int max)
        {
            var subscribers = _context.Subscribers
                .Include(s => s.Address)
                .Include(s => s.Subscription)
                .AsQueryable();

            if (max > 0)
            {
                subscribers = (Microsoft.EntityFrameworkCore.DbSet<Subscriber>)subscribers.Take(max);
            }

            return subscribers.OrderDescending().ToList();
        }

        public void Create(Subscriber subscriber, bool forceCreation = false)
        {
            // Check for existing subscribers with the same name
            SearchParams searchParams = new SearchParams
            {
                FirstName = subscriber.FirstName,
                LastName = subscriber.LastName
            };

            var existingSubscribers = Search(searchParams);

            // Prevent accidental duplicates unless explicitly overridden
            if (existingSubscribers.Count != 0 && !forceCreation)
            {
                throw new ArgumentException($"A subscriber with the name {subscriber.FirstName} {subscriber.LastName} already exists. Use forceCreation to override this check.");
            }

            _context.Add(subscriber);
            _context.SaveChanges();
        }

        public void Delete(Subscriber subscriber)
        {
            _context.Subscribers.Remove(subscriber);
            _context.SaveChanges();
        }

        public void Update(Subscriber subscriber)
        {
            _context.Update(subscriber);
            _context.SaveChanges();
        }

        public void SetIdentityInsert(bool enabled)
        {
            if (enabled)
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Subscribers ON");
            }
            else
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Subscribers OFF");
            }
        }

        public IDbContextTransaction StartTx()
        {
            return _context.Database.BeginTransaction();
        }
    }
}