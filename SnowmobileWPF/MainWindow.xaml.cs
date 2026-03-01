using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnowmobileWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISubscriberRepository _subscriberRepository;

        // Used to restore notes if user cancels edit
        private string _originalNotes = string.Empty;

        public MainWindow(ISubscriberRepository subscriberRepository)
        {
            InitializeComponent();
            _subscriberRepository = subscriberRepository;
            UpdateSubscriberList();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow();
            searchWindow.Owner = this;
            searchWindow.ShowDialog();

            if (searchWindow.DialogResult == true)
            {
                SearchParams searchParams = searchWindow.SearchParams;
                List<Subscriber>? results = _subscriberRepository.Search(searchParams);
                ClearSearchButton.Visibility = Visibility.Visible;
                SubscriberList.ItemsSource = results;
            }
        }

        private void CreateDummyButton_Click(object sender, RoutedEventArgs e)
        {
            Subscriber subscriber = new Subscriber
            {
                VSCA = new Random().Next(1, 100000),
                FirstName = "John",
                LastName = "Doe",
                Phone = "715-867-5309",
                Active = true,
                Contest = false,
                ManualMail = false,
                Commercial = false,
                DateJoined = new DateOnly(2020, 1, 1),
                Notes = "This is a dummy subscriber for testing purposes.",
                Address = new Address
                {
                    AddressId = new Random().Next(1, 100000),
                    Street = "123 Main St",
                    City = "Anytown",
                    Region = "WI",
                    PostalCode = "12345",
                    Country = "USA",
                    IsActive = true
                },
                Email = new Email
                {
                    EmailAddress = "jdoe@example.com"
                }
            };

            _subscriberRepository.Create(subscriber, true);
            UpdateSubscriberList();
        }

        private void UpdateSubscriberList()
        {
            SubscriberList.ItemsSource = null;
            SubscriberList.ItemsSource = _subscriberRepository.Retrieve(-1);
        }

        private void SubscriberList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SubscriberList.SelectedItem is Subscriber selectedSubscriber)
            {
                UpdateWindow updateWindow = new UpdateWindow(selectedSubscriber);
                updateWindow.Owner = this;
                updateWindow.ShowDialog();

                if (updateWindow.DialogResult == true)
                {
                    UpdateSubscriberList();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubscriberList.SelectedItem is Subscriber selectedSubscriber)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete {selectedSubscriber.FirstName} {selectedSubscriber.LastName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _subscriberRepository.Delete(selectedSubscriber);
                    UpdateSubscriberList();
                }
            }
            else
            {
                MessageBox.Show("Please select a subscriber to delete.",
                    "No Subscriber Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSubscriberList();
            ClearSearchButton.Visibility = Visibility.Hidden;
        }

        private void SubscriberList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubscriberList.SelectedItem is Subscriber selectedSubscriber)
            {
                ViewingTitleLabel.Content =
                    $"Viewing {selectedSubscriber.FirstName} {selectedSubscriber.LastName} (VSCA: {selectedSubscriber.VSCA})";

                DetailsPanel.Visibility = Visibility.Visible;

                // Address
                AddressLabel.Text = selectedSubscriber.Address?.Street ?? string.Empty;
                CSPLabel.Text =
                    $"{selectedSubscriber.Address?.City ?? string.Empty}, " +
                    $"{selectedSubscriber.Address?.Region ?? string.Empty} " +
                    $"{selectedSubscriber.Address?.PostalCode ?? string.Empty}"
                    .Trim()
                    .TrimStart(',');
                CountryLabel.Text = selectedSubscriber.Address?.Country ?? string.Empty;

                // Contact
                PhoneLabel.Text = selectedSubscriber.Phone ?? string.Empty;

                // Status
                ActiveCheckBox.IsChecked = selectedSubscriber.Active;
                ContestCheckBox.IsChecked = selectedSubscriber.Contest;
                ManualMailCheckBox.IsChecked = selectedSubscriber.ManualMail;
                CommercialCheckBox.IsChecked = selectedSubscriber.Commercial;

                // Subscription
                if (selectedSubscriber.Subscription != null)
                {
                    ExpirationLabel.Text =
                        $"Expires on {selectedSubscriber.Subscription.ExpDate.ToShortDateString()}";
                    RenewalLabel.Text =
                        $"Last renewed on {selectedSubscriber.Subscription.DateRenewed.ToShortDateString()}";
                    SourceLabel.Text =
                        $"Source: {selectedSubscriber.Subscription.Source}";
                }
                else
                {
                    ExpirationLabel.Text = "Expires on N/A";
                    RenewalLabel.Text = "Last renewed on N/A";
                    SourceLabel.Text = "Source: N/A";
                }

                // Notes
                NotesLabel.Text = string.IsNullOrWhiteSpace(selectedSubscriber.Notes)
                    ? "No notes."
                    : selectedSubscriber.Notes;

                ExitNotesEditMode();
            }
            else
            {
                ViewingTitleLabel.Content = "Select a subscriber...";
                DetailsPanel.Visibility = Visibility.Collapsed;

                AddressLabel.Text = string.Empty;
                CSPLabel.Text = string.Empty;
                CountryLabel.Text = string.Empty;
                PhoneLabel.Text = string.Empty;

                ActiveCheckBox.IsChecked = false;
                ContestCheckBox.IsChecked = false;
                ManualMailCheckBox.IsChecked = false;
                CommercialCheckBox.IsChecked = false;

                ExpirationLabel.Text = string.Empty;
                RenewalLabel.Text = string.Empty;
                SourceLabel.Text = string.Empty;
                NotesLabel.Text = string.Empty;

                ExitNotesEditMode();
            }
        }

        // Inline notes editing logic
        private void EditNotesButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubscriberList.SelectedItem is not Subscriber selectedSubscriber)
                return;

            _originalNotes = selectedSubscriber.Notes ?? string.Empty;
            NotesTextBox.Text = _originalNotes;

            NotesLabel.Visibility = Visibility.Collapsed;
            NotesTextBox.Visibility = Visibility.Visible;

            EditNotesButton.Visibility = Visibility.Collapsed;
            SaveNotesButton.Visibility = Visibility.Visible;
            CancelNotesButton.Visibility = Visibility.Visible;

            NotesTextBox.Focus();
        }

        private void SaveNotesButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubscriberList.SelectedItem is not Subscriber selectedSubscriber)
                return;

            selectedSubscriber.Notes = NotesTextBox.Text;
            _subscriberRepository.Update(selectedSubscriber);

            NotesLabel.Text = string.IsNullOrWhiteSpace(selectedSubscriber.Notes)
                ? "No notes."
                : selectedSubscriber.Notes;

            ExitNotesEditMode();
        }

        private void CancelNotesButton_Click(object sender, RoutedEventArgs e)
        {
            NotesTextBox.Text = _originalNotes;
            ExitNotesEditMode();
        }

        private void ExitNotesEditMode()
        {
            NotesTextBox.Visibility = Visibility.Collapsed;
            NotesLabel.Visibility = Visibility.Visible;

            EditNotesButton.Visibility = Visibility.Visible;
            SaveNotesButton.Visibility = Visibility.Collapsed;
            CancelNotesButton.Visibility = Visibility.Collapsed;
        }
    }
}