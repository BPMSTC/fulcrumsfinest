using SnowmobileLibrary.Models; // Updated to your library namespace
using SnowmobileWPF.Models;
using SnowmobileWPF.ViewModels;
using System.Windows;

namespace SnowmobileWPF
{
    public partial class SearchWindow : Window
    {
        public SearchParams SearchParams { get; private set; } = new();

        public SearchWindow()
        {
            InitializeComponent();
            DataContext = new SearchViewModel();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SearchViewModel vm)
            {
                // Extract the packaged parameters from the VM
                SearchParams = vm.GetParameters();
                DialogResult = true;
            }
        }
    }
}