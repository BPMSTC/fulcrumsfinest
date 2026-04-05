using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
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

        private void ExecuteSave()
        {
            if (YearsToRenew > 0)
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                DateOnly baseDate = CurrentSubscriber.Subscription.ExpDate < today ? today : CurrentSubscriber.Subscription.ExpDate;
                CurrentSubscriber.Subscription.ExpDate = baseDate.AddYears(YearsToRenew);
                CurrentSubscriber.Subscription.DateRenewed = DateOnly.FromDateTime(DateTime.Now);
                CurrentSubscriber.Subscription.IssuesRemaining += 4 * YearsToRenew;
                if (_contestRepository.CurrentlyInContest)
                {
                    CurrentSubscriber.Contest = true;
                }
                CloseWindow?.Invoke();
            }
        }
    }
}