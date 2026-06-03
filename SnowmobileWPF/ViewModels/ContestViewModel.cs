using Microsoft.Extensions.Logging;
using SnowmobileWPF.Repositories;
using System.Windows;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
    public class ContestViewModel : ViewModelBase
    {
        private readonly ILogger<ContestViewModel> _logger;
        private readonly IContestRepository _contestRepository;

        private DateTime _endDate;
        private DateTime _originalEndDate;

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                    IsEditing = true;
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        private bool _currentlyInContest;
        public bool CurrentlyInContest
        {
            get => _currentlyInContest;
            set => SetProperty(ref _currentlyInContest, value);
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        // Invoked after any action that modifies subscriber data, so the main window can refresh.
        public Action? OnDataChanged { get; set; }

        public ICommand StopCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand ClearContestCommand { get; }
        public ICommand ClearAdContestCommand { get; }

        public ContestViewModel(IContestRepository contestRepository, ILogger<ContestViewModel> logger)
        {
            _contestRepository = contestRepository;
            _logger = logger;

            StopCommand = new RelayCommand(_ => ExecuteStop());
            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelEditCommand = new RelayCommand(_ => ExecuteCancelEdit());
            ClearContestCommand = new RelayCommand(_ => ExecuteClearContest());
            ClearAdContestCommand = new RelayCommand(_ => ExecuteClearAdContest());

            _logger.LogInformation($"Using {this.GetType().Name}");
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            CurrentlyInContest = _contestRepository.CurrentlyInContest;
            if (CurrentlyInContest)
            {
                var endDate = _contestRepository.GetCurrentContest().EndDate;
                StatusText = $"Ends {endDate:MMM d, yyyy}";
                _endDate = endDate;
            }
            else
            {
                StatusText = string.Empty;
                _endDate = DateTime.Now;
            }

            OnPropertyChanged(nameof(EndDate));
            _originalEndDate = _endDate;
            IsEditing = false;
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

        private void ExecuteCancelEdit()
        {
            _endDate = _originalEndDate;
            OnPropertyChanged(nameof(EndDate));
            IsEditing = false;
        }

        private void ExecuteClearContest()
        {
            if (_contestRepository.CurrentlyInContest)
            {
                MessageBox.Show("Cannot clear contest entries while a contest is active. Please end the current contest first.", "Contest Active", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "This will uncheck 'Contest' for all subscribers. This cannot be undone. Are you sure?",
                "Clear Contest Entries",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                _contestRepository.ClearContestEntries();
                _logger.LogInformation("All contest entries cleared.");
                OnDataChanged?.Invoke();
                MessageBox.Show("All contest entries have been cleared.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExecuteClearAdContest()
        {
            var confirm = MessageBox.Show(
                "This will uncheck 'Ad Contest' for all subscribers. This cannot be undone. Are you sure?",
                "Clear Ad Contest Entries",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                _contestRepository.ClearAdContestEntries();
                _logger.LogInformation("All ad contest entries cleared.");
                OnDataChanged?.Invoke();
                MessageBox.Show("All ad contest entries have been cleared.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExecuteStop()
        {
            if (_contestRepository.CurrentlyInContest)
            {
                var confirmResult = MessageBox.Show("Are you sure you want to end the current contest?", "Confirm End Contest", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    _contestRepository.End();
                    UpdateStatus();
                    OnDataChanged?.Invoke();
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
