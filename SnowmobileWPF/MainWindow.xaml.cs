using System;
using System.Windows;
using System.Windows.Input;
using SnowmobileLibrary.Enums;
using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SourceComboBox.ItemsSource = Enum.GetValues(typeof(SubscriptionSource));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow
            {
                Owner = this
            };

            if (searchWindow.ShowDialog() == true)
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.LoadSearchResults(searchWindow.SearchParams);
                }
            }
        }

        private void SubscriberList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedSubscriber != null)
            {
                vm.UpdateCommand.Execute(null);
            }
        }

        private void EditNotesButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (NotesTextBox != null)
                {
                    NotesTextBox.Focus();
                    NotesTextBox.CaretIndex = NotesTextBox.Text.Length;
                }
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (DataContext is MainViewModel vm)
                {
                    if (vm.IsEditingNotes)
                    {
                        vm.CancelNotesCommand.Execute(null);
                        e.Handled = true;
                    }
                    else if (vm.SelectedSubscriber != null)
                    {
                        vm.SelectedSubscriber = null;
                        e.Handled = true;
                    }
                }
            }
        }
    }
}