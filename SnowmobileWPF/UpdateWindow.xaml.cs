using SnowmobileLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SnowmobileWPF
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private Subscriber _subscriber;
        public UpdateWindow(Subscriber subscriber)
        {
            InitializeComponent();
            _subscriber = subscriber;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_subscriber != null)
            {
                HeaderLabel.Content = $"Editing {_subscriber.ToString()}";
                FirstNameBox.Text = _subscriber.FirstName;
                LastNameBox.Text = _subscriber.LastName;
                PhoneNumberBox.Text = _subscriber.Phone;

                StreetAddressBox.Text = _subscriber.Address.Street;
                CityBox.Text = _subscriber.Address.City;
                RegionBox.Text = _subscriber.Address.Region;
                PostalCodeBox.Text = _subscriber.Address.PostalCode;
                CountryBox.Text = _subscriber.Address.Country;

                // Populate checkboxes
                ActiveCheckBox.IsChecked = _subscriber.Active;
                ContestCheckBox.IsChecked = _subscriber.Contest;
                ManualMailCheckBox.IsChecked = _subscriber.ManualMail;
                CommercialCheckBox.IsChecked = _subscriber.Commercial;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // tbd
            DialogResult = false;
        }
    }
}
