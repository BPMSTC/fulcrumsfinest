using Microsoft.EntityFrameworkCore;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Models;

namespace SnowmobileWPF.Repositories
{
    public class ContestRepository : IContestRepository
    {
        private readonly SnowmobileContext _context;

        public ContestRepository(IDbContextFactory<SnowmobileContext> factory)
        {
            _context = factory.CreateDbContext();
        }

        public bool CurrentlyInContest
        {
            get
            {
                // Evaluates if any contest record is active based on the current system time
                if (GetCurrentContest() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Create(DateTime endDate)
        {
            // Enforces business logic to prevent overlapping active contest periods
            if (CurrentlyInContest)
            {
                throw new Exception("A contest is already running.");
            }
            var newContest = new Contest
            {
                StartDate = DateTime.Now,
                EndDate = endDate
            };
            _context.Contests.Add(newContest);
            _context.SaveChanges();
        }

        public void End()
        {
            if (!CurrentlyInContest)
            {
                throw new Exception("A contest is already running.");
            }
            // Removes the active record to immediately terminate contest functionality
            _context.Contests.Remove(GetCurrentContest());
            _context.SaveChanges();

        }

        public Contest? GetCurrentContest()
        {
            // Defines an active contest as one where 'Now' is between the Start and End boundaries
            return _context.Contests
                    .Where(c => c.EndDate > DateTime.Now)
                    .Where(c => c.StartDate <= DateTime.Now)
                    .FirstOrDefault();
        }

        public bool IsLastContestAcknowledged()
        {
            // Identifies contests that have expired but haven't been "cleared" or viewed by the user yet.
            // This allows the UI to trigger notifications even after the contest has technically ended.
            var contest = _context.Contests
                    .Where(c => c.Acknowledged == false)
                    .Where(c => c.EndDate <= DateTime.Now)
                    .FirstOrDefault();
            if (contest != null)
            {
                contest.Acknowledged = true;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}