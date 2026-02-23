using SnowmobileLibrary.Models;
using SnowmobileWPF.Repositories;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            if (SubscriberList.SelectedItem != null) {
                Subscriber selectedSubscriber = (Subscriber)SubscriberList.SelectedItem;
                ViewingTitleLabel.Content = $"Viewing {SubscriberList.SelectedItem.ToString()}";
                AddressLabel.Content = selectedSubscriber.Address.Street;
                CSPLabel.Content = $"{selectedSubscriber.Address.City}, {selectedSubscriber.Address.Region} {selectedSubscriber.Address.PostalCode}";
                CountryLabel.Content = selectedSubscriber.Address.Country;

                ActiveCheckBox.IsChecked = selectedSubscriber.Active;
                ContestCheckBox.IsChecked = selectedSubscriber.Contest;
                ManualMailCheckBox.IsChecked = selectedSubscriber.ManualMail;
                CommercialCheckBox.IsChecked = selectedSubscriber.Commercial;

                ExpirationLabel.Content = $"Expires on {selectedSubscriber.Subscription.ExpDate.ToShortDateString()}";
                RenewalLabel.Content = $"Last renewed on {selectedSubscriber.Subscription.DateRenewed.ToShortDateString()}";
                SourceLabel.Content = $"Source: {selectedSubscriber.Subscription.Source}";
            }
            else
            {
                return;
            }
        }
    }
}