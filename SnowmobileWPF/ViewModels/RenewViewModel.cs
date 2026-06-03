using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
    public class RenewViewModel : ViewModelBase
    {
        private readonly ILogger<RenewViewModel> _logger;
        private readonly IContestRepository _contestRepository;

        public List<int> YearsSource { get; } = new List<int> { 1, 2, 3 };
        public int YearsToRenew { get; set; } = 1;

        public Subscriber CurrentSubscriber { get; set; }

        public string SubscriberName => $"{CurrentSubscriber?.FirstName} {CurrentSubscriber?.LastName}".Trim();
        public string CurrentExpiry => CurrentSubscriber?.Subscription?.ExpDate.ToString("MMM d, yyyy") ?? "—";

        // Invoked by code-behind to close the window after a successful save.
        public Action CloseWindow { get; set; }

        public ICommand SaveCommand { get; }

        public RenewViewModel(ILogger<RenewViewModel> logger, IContestRepository contestRepository)
        {
            _logger = logger;
            _logger.LogInformation($"Using {this.GetType().Name}");
            _contestRepository = contestRepository;
            SaveCommand = new RelayCommand(_ => ExecuteSave());
        }

        private void ExecuteSave()
        {
            if (YearsToRenew > 0)
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                DateOnly baseDate = CurrentSubscriber.Subscription.ExpDate < today
                    ? today
                    : CurrentSubscriber.Subscription.ExpDate;

                CurrentSubscriber.Subscription.ExpDate = baseDate.AddYears(YearsToRenew);
                CurrentSubscriber.Subscription.DateRenewed = DateOnly.FromDateTime(DateTime.Now);
                CurrentSubscriber.Subscription.IssuesRemaining += 4 * YearsToRenew;

                if (_contestRepository.CurrentlyInContest)
                    CurrentSubscriber.Contest = true;

                CurrentSubscriber.Active = true;
                CloseWindow?.Invoke();
            }
        }
    }
}
