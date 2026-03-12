using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using SnowmobileLibrary.Enums;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Models;
using SnowmobileWPF.Repositories;
using SnowmobileWPF.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISubscriberRepository _repository;
        private readonly ILogger<MainViewModel> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IContestRepository _contestRepository;

        private ObservableCollection<Subscriber> _subscribers = new();
        private Subscriber? _selectedSubscriber;
        private bool _isEditingNotes;
        private bool _isEditingSubscription;
        private bool _isFiltered;
        private string _notesText = string.Empty;
        private string _originalNotes = string.Empty;
        private DateTime _oldRenewDate;
        private DateTime _oldExpDate;
        private SubscriptionSource? _oldSource;
        private int? _oldIssuesRemaining;
        private string? _oldFinalIssue;
        public MainViewModel(
            ISubscriberRepository repository,
            ILogger<MainViewModel> logger,
            IServiceProvider serviceProvider, IContestRepository contestRepository)
        {
            _repository = repository;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _contestRepository = contestRepository;

            _logger.LogInformation("MainViewModel initialized.");

            // Initialize Commands
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteOnSelected);
            ImportCommand = new RelayCommand(_ => ExecuteImport());
            EditNotesCommand = new RelayCommand(_ => ExecuteEditNotes(), CanExecuteOnSelected);
            SaveNotesCommand = new RelayCommand(_ => ExecuteSaveNotes());
            CancelNotesCommand = new RelayCommand(_ => ExecuteCancelNotes());
            ClearSearchCommand = new RelayCommand(_ => ExecuteClearSearch());
            EditSubscriptionCommand = new RelayCommand(_ => ExecuteEditSubscription(), CanExecuteOnSelected);
            SaveSubscriptionCommand = new RelayCommand(_ => ExecuteSaveSubscription(), CanExecuteOnSelected);
            CancelSubscriptionCommand = new RelayCommand(_ => ExecuteCancelSubscription(), CanExecuteOnSelected);
            UpdateCommand = new RelayCommand(_ => ExecuteUpdate(), CanExecuteOnSelected);
            CreateCommand = new RelayCommand(_ => ExecuteCreate());
            ContestCommand = new RelayCommand(_ => ExecuteContest());
            RenewCommand = new RelayCommand(_ => ExecuteRenew(), CanExecuteOnSelected);
            EscapeCommand = new RelayCommand(_ => ExecuteEscape());

            // Initial load
            LoadSubscribers();
        }

        #region Properties

        public ObservableCollection<Subscriber> Subscribers
        {
            get => _subscribers;
            set => SetProperty(ref _subscribers, value);
        }

        public Subscriber? SelectedSubscriber
        {
            get => _selectedSubscriber;
            set
            {
                if (SetProperty(ref _selectedSubscriber, value))
                {
                    _logger.LogDebug("SelectedSubscriber changed to VSCA: {VSCA}", value?.VSCA);
                    // Reset UI state when a new subscriber is selected
                    IsEditingNotes = false;
                    UpdateNotesDisplay();
                    UpdateSubscriptionDisplay();
                    OnPropertyChanged(nameof(IsDetailsVisible));
                    OnPropertyChanged(nameof(ViewingTitle));
                }
            }
        }

        public bool IsDetailsVisible => SelectedSubscriber != null;

        public bool IsListEmpty => Subscribers == null || Subscribers.Count == 0;

        public string ViewingTitle => SelectedSubscriber != null
            ? $"Viewing {SelectedSubscriber.FirstName} {SelectedSubscriber.LastName} (VSCA: {SelectedSubscriber.VSCA})"
            : "Select a subscriber...";

        public bool IsEditingNotes
        {
            get => _isEditingNotes;
            set => SetProperty(ref _isEditingNotes, value);
        }

        public bool IsEditingSubscription
        {
            get => _isEditingSubscription;
            set => SetProperty(ref _isEditingSubscription, value);
        }

        public bool IsFiltered
        {
            get => _isFiltered;
            set => SetProperty(ref _isFiltered, value);
        }

        public string NotesText
        {
            get => _notesText;
            set => SetProperty(ref _notesText, value);
        }

        public DateTime RenewDate
        {
            get => _oldRenewDate;
            set => SetProperty(ref _oldRenewDate, value);
        }

        public DateTime ExpDate
        {
            get => _oldExpDate;
            set => SetProperty(ref _oldExpDate, value);
        }

        public SubscriptionSource? Source
        {
            get => _oldSource;
            set => SetProperty(ref _oldSource, value);
        }

        public int? IssuesRemaining
        {
            get => _oldIssuesRemaining;
            set => SetProperty(ref _oldIssuesRemaining, value);
        }

        public string? FinalIssue
        {
            get => _oldFinalIssue;
            set => SetProperty(ref _oldFinalIssue, value);
        }

        #endregion

        #region Commands

        public ICommand DeleteCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand EditNotesCommand { get; }
        public ICommand SaveNotesCommand { get; }
        public ICommand CancelNotesCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand EditSubscriptionCommand { get; }
        public ICommand SaveSubscriptionCommand { get; }
        public ICommand CancelSubscriptionCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EscapeCommand { get; }
        public ICommand ContestCommand { get; }
        public ICommand RenewCommand { get; }

        #endregion

        #region Logic Methods

        internal bool CheckAcknowledged => _contestRepository.IsLastContestAcknowledged();

        public void RefreshDisplay()
        {
            _logger.LogDebug("Refreshing UI display for SelectedSubscriber.");
            // Forces UI elements bound to the object or its strings to re-evaluate
            OnPropertyChanged(nameof(SelectedSubscriber));
            OnPropertyChanged(nameof(ViewingTitle));
            UpdateNotesDisplay();
            UpdateSubscriptionDisplay();
        }

        private void UpdateNotesDisplay()
        {
            NotesText = string.IsNullOrWhiteSpace(SelectedSubscriber?.Notes)
                ? "No notes."
                : SelectedSubscriber.Notes;
        }

        private void UpdateSubscriptionDisplay()
        {
            var sub = SelectedSubscriber?.Subscription;
            if (sub == null)
            {
                RenewDate = DateTime.Today;
                ExpDate = DateTime.Today.AddYears(1);
                Source = null;
                IssuesRemaining = 0;
                return;
            }

            RenewDate = sub.DateRenewed.ToDateTime(new TimeOnly(0));
            ExpDate = sub.ExpDate.ToDateTime(new TimeOnly(0));
            Source = sub.Source;
            IssuesRemaining = sub.IssuesRemaining;
            FinalIssue = sub.FinalIssue;
        }

        public void LoadSubscribers()
        {
            _logger.LogInformation("Loading subscribers from repository.");
            var results = _repository.Retrieve(-1);
            Subscribers = new ObservableCollection<Subscriber>(results);

            IsFiltered = false;
            OnPropertyChanged(nameof(IsListEmpty));
        }

        public List<Subscriber> LoadSearchResults(SearchParams searchParameters)
        {
            _logger.LogInformation("Loading search results into UI.");
            var results = _repository.Search(searchParameters) ?? new List<Subscriber>();
            Subscribers = new ObservableCollection<Subscriber>(results);

            IsFiltered = true;
            OnPropertyChanged(nameof(IsListEmpty));

            return results;
        }

        private bool CanExecuteOnSelected(object? parameter) => SelectedSubscriber != null;

        private void ExecuteCreate()
        {
            _logger.LogInformation("Opening Create Window for new subscriber.");

            Subscriber newSubscriber = new Subscriber
            {
                Address = new Address(),
                Subscription = new Subscription
                {
                    ExpDate = DateOnly.FromDateTime(DateTime.Today),
                    DateRenewed = DateOnly.FromDateTime(DateTime.Today)
                }
            };

            var createLogger = _serviceProvider.GetRequiredService<ILogger<UpdateViewModel>>();

            var vm = new UpdateViewModel(newSubscriber, _repository, createLogger);

            var createWin = new UpdateWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (createWin.ShowDialog() == true)
            {
                _logger.LogInformation("Create Window saved new subscriber");

                _repository.Create(newSubscriber, true);

                LoadSubscribers();
                SelectedSubscriber = newSubscriber;
            }
        }

        private void ExecuteUpdate()
        {
            if (SelectedSubscriber == null) return;

            _logger.LogInformation("Opening Update Window for VSCA: {VSCA}", SelectedSubscriber.VSCA);

            // Get a logger for the UpdateViewModel
            var updateLogger = _serviceProvider.GetRequiredService<ILogger<UpdateViewModel>>();

            // Create the ViewModel with the selected data
            var vm = new UpdateViewModel(SelectedSubscriber, _repository, updateLogger);

            // Create and show the window
            var updateWin = new UpdateWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (updateWin.ShowDialog() == true)
            {
                _logger.LogInformation("Update Window saved changes for VSCA: {VSCA}", SelectedSubscriber.VSCA);
                _repository.Update(SelectedSubscriber);
                RefreshDisplay();
            }
            else
            {
                _logger.LogInformation("Update Window cancelled for VSCA: {VSCA}", SelectedSubscriber.VSCA);
            }
        }

        private void ExecuteDelete(object? parameter)
        {
            if (SelectedSubscriber == null) return;

            _logger.LogWarning("User initiated delete for VSCA: {VSCA}", SelectedSubscriber.VSCA);

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedSubscriber.FirstName} {SelectedSubscriber.LastName}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _repository.Delete(SelectedSubscriber);
                _logger.LogInformation("Successfully deleted VSCA: {VSCA}", SelectedSubscriber.VSCA);
                LoadSubscribers();
                SelectedSubscriber = null;
            }
            else
            {
                _logger.LogInformation("Delete operation cancelled for VSCA: {VSCA}", SelectedSubscriber.VSCA);
            }
        }

        private void ExecuteEditSubscription()
        {
            _logger.LogInformation("Edit Subscription command executed for VSCA: {VSCA}", SelectedSubscriber?.VSCA);

            RenewDate = SelectedSubscriber.Subscription.DateRenewed.ToDateTime(new TimeOnly(0));
            ExpDate = SelectedSubscriber.Subscription.ExpDate.ToDateTime(new TimeOnly(0));
            Source = SelectedSubscriber?.Subscription.Source;
            IssuesRemaining = SelectedSubscriber.Subscription.IssuesRemaining;

            IsEditingSubscription = true;
        }

        private void ExecuteSaveSubscription()
        {
            if (SelectedSubscriber == null) return;

            _logger.LogInformation("Saving updated subscription for VSCA: {VSCA}", SelectedSubscriber.VSCA);

            SelectedSubscriber.Subscription.ExpDate = DateOnly.FromDateTime(ExpDate);
            SelectedSubscriber.Subscription.DateRenewed = DateOnly.FromDateTime(RenewDate);
            SelectedSubscriber.Subscription.Source = Source;
            SelectedSubscriber.Subscription.IssuesRemaining = IssuesRemaining ?? 0;

            _repository.Update(SelectedSubscriber);

            IsEditingSubscription = false;
            UpdateSubscriptionDisplay();
        }

        private void ExecuteCancelSubscription()
        {
            _logger.LogInformation("Cancelled subscription edit for VSCA: {VSCA}", SelectedSubscriber?.VSCA);
            IsEditingSubscription = false;
            UpdateSubscriptionDisplay();
        }

        private void ExecuteEditNotes()
        {
            _logger.LogDebug("Entering Notes Edit mode for VSCA: {VSCA}", SelectedSubscriber?.VSCA);
            _originalNotes = SelectedSubscriber?.Notes ?? string.Empty;
            NotesText = _originalNotes;
            IsEditingNotes = true;
        }

        private void ExecuteSaveNotes()
        {
            if (SelectedSubscriber == null) return;

            _logger.LogInformation("Saving updated notes for VSCA: {VSCA}", SelectedSubscriber.VSCA);
            _logger.LogDebug("Old Note: {Old} | New Note: {New}", _originalNotes, NotesText);

            SelectedSubscriber.Notes = NotesText;
            _repository.Update(SelectedSubscriber);
            IsEditingNotes = false;
            UpdateNotesDisplay();
        }

        private void ExecuteCancelNotes()
        {
            _logger.LogDebug("Cancelled notes edit for VSCA: {VSCA}", SelectedSubscriber?.VSCA);
            IsEditingNotes = false;
            UpdateNotesDisplay();
        }

        private void ExecuteContest()
        {
            _logger.LogDebug("Opening contest window");
            var contestRepo = _serviceProvider.GetRequiredService<IContestRepository>();
            ContestWindow contestWindow = new ContestWindow(
                _serviceProvider.GetRequiredService<ContestViewModel>()
                );
            contestWindow.Show();
        }

        private void ExecuteRenew()
        {
            _logger.LogDebug("Opening renew window");
            RenewWindow renewWindow = new RenewWindow(
                _serviceProvider.GetRequiredService<RenewViewModel>(),
                SelectedSubscriber
            );
            renewWindow.ShowDialog();
            if (renewWindow.DialogResult == true)
            {
                _logger.LogInformation("Renewal completed for VSCA: {VSCA}", SelectedSubscriber?.VSCA);
                ExecuteCancelSubscription();
                OnPropertyChanged(nameof(SelectedSubscriber));
            }
            else
            {
                _logger.LogInformation("Renewal cancelled for VSCA: {VSCA}", SelectedSubscriber?.VSCA);
            }
        }

        private async void ExecuteImport()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                _logger.LogInformation($"Starting import from {fileDialog.FileName}");
                CSVImportService importService = new CSVImportService(_repository);
                LoadingWindow loadingWindow = new LoadingWindow();
                loadingWindow.Show();
                await importService.ImportCSV(fileDialog.FileName, loadingWindow.progress);
                loadingWindow.Close();
                _logger.LogInformation($"Import complete.");
                LoadSubscribers();
            }
        }

        private void ExecuteEscape()
        {
            // If editing subscription, cancel that first
            if (IsEditingSubscription)
            {
                ExecuteCancelSubscription();
                return;
            }

            // If editing notes, cancel that
            if (IsEditingNotes)
            {
                ExecuteCancelNotes();
                return;
            }

            // If not editing anything, deselect the subscriber (close the "tab")
            if (SelectedSubscriber != null)
            {
                SelectedSubscriber = null;
            }
        }

        private void ExecuteClearSearch()
        {
            LoadSubscribers();
        }

        #endregion
    }
}