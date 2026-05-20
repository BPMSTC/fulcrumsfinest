using Microsoft.EntityFrameworkCore;
using SnowmobileLibrary.Data;
using SnowmobileLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
                // checks if a contest is currently running
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
            // prevent running if there's no running contest
            if (!CurrentlyInContest)
            {
                throw new Exception("A contest is already running.");
            }
            _context.Contests.Remove(GetCurrentContest());
            _context.SaveChanges();

        }

        public void ClearContestEntries()
        {
            var contestants = _context.Subscribers
                .Where(s => s.Contest)
                .ToList();

            foreach (var subscriber in contestants)
                subscriber.Contest = false;

            _context.SaveChanges();
        }

        public Contest? GetCurrentContest()
        {
            return _context.Contests
                    .Where(c => c.EndDate > DateTime.Now)
                    .Where(c => c.StartDate <= DateTime.Now)
                    .FirstOrDefault();
        }

        public bool IsLastContestAcknowledged()
        {
            var contest = _context.Contests
                    .Where(c => c.Acknowledged == false)
                    .Where(c => c.EndDate <= DateTime.Now)
                    .FirstOrDefault();
            if (contest != null)
            {
                contest.Acknowledged = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
