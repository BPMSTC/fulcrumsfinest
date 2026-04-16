using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;

namespace SnowmobileWPF.Repositories
{
    /// <summary>
    /// A mock implementation of the contest repository for development and UI testing.
    /// Provides predictable behavior without requiring a live database connection.
    /// </summary>
    public class LocalContestRepository : IContestRepository
    {
        private readonly ILogger<LocalSubscriberRepository> _logger;

        // Simplified state management for testing; does not persist across application restarts
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
            // Returns a dummy contest object to ensure UI components have data to bind to
            return new Contest
            {
                EndDate = DateTime.Now.AddDays(7)
            };
        }

        public bool IsLastContestAcknowledged()
        {
            return false;
        }
    }
}