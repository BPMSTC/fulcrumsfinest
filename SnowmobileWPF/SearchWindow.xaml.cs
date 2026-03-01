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

        public SearchWindow()
        {
            InitializeComponent();

            var logger = App.AppHost.Services.GetRequiredService<ILogger<SearchViewModel>>();
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
    }
}