using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;

namespace SnowmobileWPF.Repositories
{
    public class LocalContestRepository : IContestRepository
    {
        private readonly ILogger<LocalSubscriberRepository> _logger;

        // no date here since it's just for testing
        private bool _currentlyInContest;

        public bool CurrentlyInContest 
        {
            get => _currentlyInContest;
        }

        public LocalContestRepository(ILogger<LocalSubscriberRepository> logger)
        {
            _logger = logger;
            _logger.LogInformation($"Using {this.GetType().Name}");
        }


        public void Create(DateTime endDate)
        {
            _logger.LogInformation("Starting new contest");
            _currentlyInContest = true;
        }

        public void End()
        {
            _logger.LogInformation("Ending current contest");
            _currentlyInContest = false;
        }

        public Contest? GetCurrentContest()
        {
            return new Contest
            {
                EndDate = DateTime.Now.AddDays(7)
            };
        }

        public void ClearContestEntries()
        {
            _logger.LogInformation("Clearing all contest entries (local stub — no-op).");
        }

        public bool IsLastContestAcknowledged()
        {
            return false;
        }
    }
}
