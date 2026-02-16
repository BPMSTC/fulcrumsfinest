using SnowmobileLibrary.Models;
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
        private List<Subscriber> subscribers = new List<Subscriber>();
        public MainWindow()
        {
            InitializeComponent();
            Subscriber subscriber = new Subscriber
            {
                VSCA = 12345,
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
            subscribers.Add(subscriber);
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
                List<Subscriber> results = subscribers;
                if (searchParams.VSCA != null)
                {
                    results = results.Where(s => s.VSCA == searchParams.VSCA).ToList();
                }
                if (!string.IsNullOrEmpty(searchParams.FirstName))
                {
                    results = results.Where(s => s.FirstName.Contains(searchParams.FirstName, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!string.IsNullOrEmpty(searchParams.LastName))
                {
                    results = results.Where(s => s.LastName.Contains(searchParams.LastName, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!string.IsNullOrEmpty(searchParams.PhoneNumber))
                {
                    results = results.Where(s => s.Phone.Contains(searchParams.PhoneNumber)).ToList();
                }
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
            subscribers.Add(subscriber);
            UpdateSubscriberList();
        }

        private void UpdateSubscriberList()
        {
            SubscriberList.ItemsSource = null;
            SubscriberList.ItemsSource = subscribers;
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
                    // Update the subscriber in the list
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
                    subscribers.Remove(selectedSubscriber);
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
    }
}