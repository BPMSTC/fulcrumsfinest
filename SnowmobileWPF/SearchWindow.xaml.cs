using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SnowmobileWPF.Models;
using SnowmobileWPF.ViewModels;
using System.Windows;

namespace SnowmobileWPF
{
    public partial class SearchWindow : Window
    {
        public SearchParams SearchParams { get; private set; } = new();
        // necessary for live searching
        private readonly MainViewModel _mainViewModel;

        public SearchWindow()
        {
            InitializeComponent();

            var logger = App.AppHost.Services.GetRequiredService<ILogger<SearchViewModel>>();
            _mainViewModel = App.AppHost.Services.GetRequiredService<MainViewModel>();
            DataContext = new SearchViewModel(logger);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SearchViewModel vm)
            {
                SearchParams = vm.GetParameters();
                DialogResult = true;
            }
        }

        private void UpdateSearchResults(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (DataContext is SearchViewModel vm)
            {
                SearchParams = vm.GetParameters();
                _mainViewModel.LoadSearchResults(SearchParams);
            }
        }
    }
}