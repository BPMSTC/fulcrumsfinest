using SnowmobileLibrary.Models;
using SnowmobileWPF.Models;
using SnowmobileWPF.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SnowmobileWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISubscriberRepository _repository;
        private ObservableCollection<Subscriber> _subscribers = new();
        private Subscriber? _selectedSubscriber;
        private bool _isEditingNotes;
        private string _notesText = string.Empty;
        private string _originalNotes = string.Empty;

        public MainViewModel(ISubscriberRepository repository)
        {
            _repository = repository;

            // Initialize Commands
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteOnSelected);
            CreateDummyCommand = new RelayCommand(_ => ExecuteCreateDummy());
            EditNotesCommand = new RelayCommand(_ => ExecuteEditNotes(), CanExecuteOnSelected);
            SaveNotesCommand = new RelayCommand(_ => ExecuteSaveNotes());
            CancelNotesCommand = new RelayCommand(_ => ExecuteCancelNotes());

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
                    // Reset UI state when a new subscriber is selected
                    IsEditingNotes = false;
                    UpdateNotesDisplay();
                    OnPropertyChanged(nameof(IsDetailsVisible));
                    OnPropertyChanged(nameof(ViewingTitle));
                }
            }
        }

        public bool IsDetailsVisible => SelectedSubscriber != null;

        public string ViewingTitle => SelectedSubscriber != null
            ? $"Viewing {SelectedSubscriber.FirstName} {SelectedSubscriber.LastName} (VSCA: {SelectedSubscriber.VSCA})"
            : "Select a subscriber...";

        public bool IsEditingNotes
        {
            get => _isEditingNotes;
            set => SetProperty(ref _isEditingNotes, value);
        }

        public string NotesText
        {
            get => _notesText;
            set => SetProperty(ref _notesText, value);
        }

        #endregion

        #region Commands

        public ICommand DeleteCommand { get; }
        public ICommand CreateDummyCommand { get; }
        public ICommand EditNotesCommand { get; }
        public ICommand SaveNotesCommand { get; }
        public ICommand CancelNotesCommand { get; }

        #endregion

        #region Logic Methods

        public void RefreshDisplay()
        {
            // Forces UI elements bound to the object or its strings to re-evaluate
            OnPropertyChanged(nameof(SelectedSubscriber));
            OnPropertyChanged(nameof(ViewingTitle));
            UpdateNotesDisplay();
        }

        private void UpdateNotesDisplay()
        {
            NotesText = string.IsNullOrWhiteSpace(SelectedSubscriber?.Notes)
                ? "No notes."
                : SelectedSubscriber.Notes;
        }

        public void LoadSubscribers()
        {
            var results = _repository.Retrieve(-1);
            Subscribers = new ObservableCollection<Subscriber>(results);
        }

        public List<Subscriber> LoadSearchResults(SearchParams searchParameters)
        {
            var results = _repository.Search(searchParameters);
            Subscribers = new ObservableCollection<Subscriber>(results);
            return results;
        }

        private bool CanExecuteOnSelected(object? parameter) => SelectedSubscriber != null;

        private void ExecuteDelete(object? parameter)
        {
            if (SelectedSubscriber == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedSubscriber.FirstName} {SelectedSubscriber.LastName}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _repository.Delete(SelectedSubscriber);
                LoadSubscribers();
                SelectedSubscriber = null;
            }
        }

        private void ExecuteCreateDummy()
        {
            var dummy = new Subscriber
            {
                VSCA = new Random().Next(1, 100000),
                FirstName = "John",
                LastName = "Doe",
                Phone = "715-555-0199",
                Active = true,
                DateJoined = DateOnly.FromDateTime(DateTime.Now),
                Address = new Address { Street = "123 MVVM Way", City = "Pattern", Region = "WI", Country = "USA", PostalCode = "16823" }
            };

            _repository.Create(dummy, true);
            LoadSubscribers();
        }

        private void ExecuteEditNotes()
        {
            // If it currently says "No notes.", we want the editor to start empty
            _originalNotes = SelectedSubscriber?.Notes ?? string.Empty;
            NotesText = _originalNotes;
            IsEditingNotes = true;
        }

        private void ExecuteSaveNotes()
        {
            if (SelectedSubscriber == null) return;

            SelectedSubscriber.Notes = NotesText;
            _repository.Update(SelectedSubscriber);
            IsEditingNotes = false;
            UpdateNotesDisplay(); // Refresh display to show the new note (or "No notes.")
        }

        private void ExecuteCancelNotes()
        {
            IsEditingNotes = false;
            UpdateNotesDisplay(); // Revert back to original note or placeholder
        }

        #endregion
    }
}