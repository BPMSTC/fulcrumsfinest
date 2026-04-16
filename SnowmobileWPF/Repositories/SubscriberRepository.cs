using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Models;

namespace SnowmobileWPF.Repositories
{
    /// <summary>
    /// Handles production database operations for Subscriber entities using Entity Framework Core.
    /// </summary>
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly SnowmobileContext _context;

        public SubscriberRepository(IDbContextFactory<SnowmobileContext> factory)
        {
            _context = factory.CreateDbContext();
        }

        public List<Subscriber>? Search(SearchParams searchParams)
        {
            // .Include() performs Eager Loading to fetch related Address and Subscription 
            // data in a single SQL JOIN, preventing the N+1 performance issue.
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
                // Explicit cast ensures we maintain the correctly typed collection after truncation
                subscribers = (Microsoft.EntityFrameworkCore.DbSet<Subscriber>)subscribers.Take(max);
            }

            return subscribers.OrderDescending().ToList();
        }

        public void Create(Subscriber subscriber, bool forceCreation = false)
        {
            SearchParams searchParams = new SearchParams
            {
                FirstName = subscriber.FirstName,
                LastName = subscriber.LastName
            };

            var existingSubscribers = Search(searchParams);

            // Validation logic to prevent accidental duplicates during manual entry,
            // while allowing the 'force' flag for bulk migrations/imports.
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
            // Marks the entity as modified and pushes only changed properties to the database
            _context.Update(subscriber);
            _context.SaveChanges();
        }

        public void SetIdentityInsert(bool enabled)
        {
            // Direct SQL execution to override SQL Server's automatic ID generation.
            // Essential when importing legacy data where IDs (VSCA numbers) must be preserved.
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
            // Provides an atomic transaction to ensure data integrity during multi-step operations (like bulk imports)
            return _context.Database.BeginTransaction();
        }
    }
}