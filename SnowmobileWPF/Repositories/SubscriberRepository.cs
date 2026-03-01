using Microsoft.EntityFrameworkCore;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Models;

namespace SnowmobileWPF.Repositories
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly SnowmobileContext _context;

        public SubscriberRepository(SnowmobileContext context)
        {
            _context = context;
        }

        public List<Subscriber>? Search(SearchParams searchParams)
        {
            var query = _context.Subscribers
                .Include(s => s.Address)
                .Include(s => s.Subscription)
                .Include(s => s.Email)
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
                .Include(s => s.Email)
                .AsQueryable();
            if (max > 0)
            {
                subscribers = (Microsoft.EntityFrameworkCore.DbSet<Subscriber>)subscribers.Take(max);
            }
            return subscribers.ToList();
        }

        public void Create(Subscriber subscriber, bool forceCreation = false)
        {
            // todo: check for existing subscriber if forceCreation is false
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
            _context.Subscribers.Update(subscriber);
            _context.SaveChanges();
        }
    }
}
