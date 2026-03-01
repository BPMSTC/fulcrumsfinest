using System;
using System.Windows;
using System.Windows.Input;
using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow();
            searchWindow.Owner = this;

            if (searchWindow.ShowDialog() == true)
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.LoadSearchResults(searchWindow.SearchParams);
                    ClearSearchButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadSubscribers();
                ClearSearchButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SubscriberList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedSubscriber != null)
            {
                UpdateWindow updateWindow = new UpdateWindow(vm.SelectedSubscriber);
                updateWindow.Owner = this;

                if (updateWindow.ShowDialog() == true)
                {
                    SubscriberList.Items.Refresh();
                    vm.RefreshDisplay();
                }
            }
        }

        /// <summary>
        /// Handles the focus logic when the user enters Edit mode for notes.
        /// </summary>
        private void EditNotesButton_Click(object sender, RoutedEventArgs e)
        {
            // We use the Dispatcher to wait until the UI thread has updated the 
            // TextBox visibility before trying to set focus.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (NotesTextBox != null)
                {
                    NotesTextBox.Focus();
                    // Moves the cursor to the end of the text
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