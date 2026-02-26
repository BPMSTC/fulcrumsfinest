using SnowmobileLibrary.Models;
using System.Windows;

namespace SnowmobileWPF
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private readonly Subscriber _subscriber;

        public UpdateWindow(Subscriber subscriber)
        {
            InitializeComponent();
            _subscriber = subscriber;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_subscriber == null)
                return;

            // Header
            HeaderLabel.Text = $"Editing {_subscriber.ToString()}";

            // Basic Info
            FirstNameBox.Text = _subscriber.FirstName;
            LastNameBox.Text = _subscriber.LastName;
            PhoneNumberBox.Text = _subscriber.Phone;

            // Address
            StreetAddressBox.Text = _subscriber.Address.Street;
            CityBox.Text = _subscriber.Address.City;
            RegionBox.Text = _subscriber.Address.Region;
            PostalCodeBox.Text = _subscriber.Address.PostalCode;
            CountryBox.Text = _subscriber.Address.Country;

            // Issues
            //EmailBox.Text = _subscriber.IssuesLeft.ToString(CultureInfo.InvariantCulture);

            // Status Flags
            ActiveCheckBox.IsChecked = _subscriber.Active;
            ContestCheckBox.IsChecked = _subscriber.Contest;
            ManualMailCheckBox.IsChecked = _subscriber.ManualMail;
            CommercialCheckBox.IsChecked = _subscriber.Commercial;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSubscriber();
            DialogResult = true;
            Close();
        }

        private void UpdateSubscriber()
        {
            // Basic Info
            _subscriber.FirstName = FirstNameBox.Text.Trim();
            _subscriber.LastName = LastNameBox.Text.Trim();
            _subscriber.Phone = PhoneNumberBox.Text.Trim();
            //_subscriber.Email = EmailBox.Text.Trim();

            // Address
            _subscriber.Address.Street = StreetAddressBox.Text.Trim();
            _subscriber.Address.City = CityBox.Text.Trim();
            _subscriber.Address.Region = RegionBox.Text.Trim();
            _subscriber.Address.PostalCode = PostalCodeBox.Text.Trim();
            _subscriber.Address.Country = CountryBox.Text.Trim();

            // Status Flags
            _subscriber.Active = ActiveCheckBox.IsChecked ?? false;
            _subscriber.Contest = ContestCheckBox.IsChecked ?? false;
            _subscriber.ManualMail = ManualMailCheckBox.IsChecked ?? false;
            _subscriber.Commercial = CommercialCheckBox.IsChecked ?? false;
        }
    }
}