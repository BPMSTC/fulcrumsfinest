using Microsoft.Extensions.Logging;
using SnowmobileWPF.Repositories;
using System.Windows;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
    /// <summary>
    /// Manages the lifecycle of promotional contests.
    /// Acts as the coordinator between the Contest Repository and the UI, handling validation 
    /// to ensure only one contest is active at any given time.
    /// </summary>
    public class ContestViewModel : ViewModelBase
    {
        private readonly ILogger<ContestViewModel> _logger;
        private readonly IContestRepository _contestRepository;

        private DateTime _endDate;

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                SetProperty(ref _endDate, value);
            }
        }

        private bool _currentlyInContest;
        public bool CurrentlyInContest
        {
            get => _currentlyInContest;
            set
            {
                SetProperty(ref _currentlyInContest, value);
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set
            {
                SetProperty(ref _statusText, value);
            }
        }

        public ICommand StopCommand { get; }
        public ICommand SaveCommand { get; }

        public ContestViewModel(IContestRepository contestRepository, ILogger<ContestViewModel> logger)
        {
            StopCommand = new RelayCommand(_ => ExecuteStop());
            SaveCommand = new RelayCommand(_ => ExecuteSave());
            _contestRepository = contestRepository;
            UpdateStatus();
            _logger = logger;
            _logger.LogInformation($"Using {this.GetType().Name}");
        }

        /// <summary>
        /// Synchronizes the View's state with the repository.
        /// Determines the UI visibility and text feedback based on whether a contest record is currently active.
        /// </summary>
        private void UpdateStatus()
        {
            CurrentlyInContest = _contestRepository.CurrentlyInContest;
            if (CurrentlyInContest)
            {
                var endDate = _contestRepository.GetCurrentContest().EndDate;
                StatusText = $"Active until {_contestRepository.GetCurrentContest().EndDate}";
                EndDate = endDate;
            }
            else
            {
                StatusText = "Inactive";
                EndDate = DateTime.Now;
            }
        }

        private void ExecuteSave()
        {
            if (_contestRepository.CurrentlyInContest)
            {
                MessageBox.Show("A contest is already in progress. Please end the current contest before starting a new one.", "Contest In Progress", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                _contestRepository.Create(EndDate);
                UpdateStatus();
                MessageBox.Show($"Contest started and will end on {EndDate}.", "Contest Started", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Logic for manually terminating a contest. 
        /// Includes a confirmation safety check to prevent accidental closure of active promotions.
        /// </summary>
        private void ExecuteStop()
        {
            if (_contestRepository.CurrentlyInContest)
            {
                var confirmResult = MessageBox.Show("Are you sure you want to end the current contest?", "Confirm End Contest", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    _contestRepository.End();
                    UpdateStatus();
                    MessageBox.Show("Contest ended.", "Contest Ended", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No contest is currently in progress.", "No Contest", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}