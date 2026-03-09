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

        public ContestRepository(SnowmobileContext context)
        {
            _context = context;
        }

        public bool CurrentlyInContest
        {
            get
            {
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
            var newContest = new SnowmobileLibrary.Models.Contest
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
            _context.Contests.Remove(GetCurrentContest());

        }

        public Contest? GetCurrentContest()
        {
            return _context.Contests
                    .Where(c => c.EndDate > DateTime.Now)
                    .Where(c => c.StartDate <= DateTime.Now)
                    .FirstOrDefault();
        }
    }
}
