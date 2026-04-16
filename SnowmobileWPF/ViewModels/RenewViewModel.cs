using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
    /// <summary>
    /// Manages the renewal process for existing subscribers.
    /// It calculates new expiration dates based on the current subscription status and 
    /// automatically enrolls subscribers in active contests upon renewal.
    /// </summary>
    public class RenewViewModel : ViewModelBase
    {
        public List<int> YearsSource { get; } = new List<int> { 1, 2, 3 };
        private readonly ILogger<ContestViewModel> _logger;
        private readonly IContestRepository _contestRepository;
        public int YearsToRenew { get; set; } = 1;
        public Subscriber CurrentSubscriber { get; set; }

        // points to code-behind to close window
        public Action CloseWindow { get; set; }

        public RenewViewModel(ILogger<ContestViewModel> logger, IContestRepository contestRepository)
        {
            _logger = logger;
            _logger.LogInformation($"Using {this.GetType().Name}");
            _contestRepository = contestRepository;
            SaveCommand = new RelayCommand(param => ExecuteSave());
        }

        public ICommand SaveCommand { get; }

        /// <summary>
        /// Executes the renewal logic. 
        /// If a subscription is already expired, the new term starts from today; 
        /// otherwise, the new years are appended to the existing expiration date to prevent time loss.
        /// </summary>
        private void ExecuteSave()
        {
            if (YearsToRenew > 0)
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                // Logic to determine the "Anchor" date for renewal:
                // Prevents renewals from starting in the past if the subscriber is lapsed.
                DateOnly baseDate = CurrentSubscriber.Subscription.ExpDate < today ? today : CurrentSubscriber.Subscription.ExpDate;

                CurrentSubscriber.Subscription.ExpDate = baseDate.AddYears(YearsToRenew);
                CurrentSubscriber.Subscription.DateRenewed = DateOnly.FromDateTime(DateTime.Now);

                // Standard business rule: 4 issues per year of subscription.
                CurrentSubscriber.Subscription.IssuesRemaining += 4 * YearsToRenew;

                // Cross-module integration: Automatically marks the subscriber as a contest entrant 
                // if the renewal occurs during an active promotional period.
                if (_contestRepository.CurrentlyInContest)
                {
                    CurrentSubscriber.Contest = true;
                }

                CloseWindow?.Invoke();
            }
        }
    }
}