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
    /// A set of parameters to be used for searching subscribers. All fields are optional, and the search will match any subscriber that matches all non-null fields.
    /// </summary>
    public class SearchParams
    {
        public int? VSCA { get; set; }
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchParams SearchParams { get; private set; }

        public SearchWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchParams = new SearchParams
            {
                LastName = LastNameTextBox.Text,
                FirstName = FirstNameTextBox.Text,
                PhoneNumber = PhoneNumberTextBox.Text,
                VSCA = int.TryParse(VSCATextBox.Text, out int vsca) ? vsca : null
            };
            DialogResult = true;
        }
    }
}
