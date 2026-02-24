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

            // if user initiates a search
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
            // check if an item is selected and cast it to a Subscriber
            if (SubscriberList.SelectedItem is Subscriber selectedSubscriber)
            {
                UpdateWindow updateWindow = new UpdateWindow(selectedSubscriber);
                updateWindow.Owner = this;
                updateWindow.ShowDialog();

                // if user clicks update, refresh the list to show any changes
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
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {selectedSubscriber.FirstName} {selectedSubscriber.LastName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _subscriberRepository.Delete(selectedSubscriber);
                    UpdateSubscriberList();
                }
            } else
            {
                MessageBox.Show("Please select a subscriber to delete.", "No Subscriber Selected", MessageBoxButton.OK, MessageBoxImage.Information);
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
                ViewingTitleLabel.Content = $"Viewing {SubscriberList.SelectedItem}";

                // Handles potential null Address
                AddressLabel.Content = selectedSubscriber.Address?.Street ?? string.Empty;
                CSPLabel.Content =
                    $"{selectedSubscriber.Address?.City ?? string.Empty}, " +
                    $"{selectedSubscriber.Address?.Region ?? string.Empty} " +
                    $"{selectedSubscriber.Address?.PostalCode ?? string.Empty}"
                    .Trim()
                    .TrimStart(',');
                CountryLabel.Content = selectedSubscriber.Address?.Country ?? string.Empty;

                ActiveCheckBox.IsChecked = selectedSubscriber.Active;
                ContestCheckBox.IsChecked = selectedSubscriber.Contest;
                ManualMailCheckBox.IsChecked = selectedSubscriber.ManualMail;
                CommercialCheckBox.IsChecked = selectedSubscriber.Commercial;

                // Handles potential null Subscription
                if (selectedSubscriber.Subscription != null)
                {
                    ExpirationLabel.Content = $"Expires on {selectedSubscriber.Subscription.ExpDate.ToShortDateString()}";
                    RenewalLabel.Content = $"Last renewed on {selectedSubscriber.Subscription.DateRenewed.ToShortDateString()}";
                    SourceLabel.Content = $"Source: {selectedSubscriber.Subscription.Source}";
                }
                else
                {
                    ExpirationLabel.Content = "Expires on N/A";
                    RenewalLabel.Content = "Last renewed on N/A";
                    SourceLabel.Content = "Source: N/A";
                }

                // Shows "No Notes." if Notes is null or whitespace, otherwise shows the notes
                NotesLabel.Content = string.IsNullOrWhiteSpace(selectedSubscriber.Notes)
                    ? "No Notes."
                    : $"Notes: {selectedSubscriber.Notes}";
            }
            else
            {
                return;
            }
        }
    }
}