using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using SnowmobileLibrary.Enums;
using SnowmobileWPF.ViewModels;
using System.Windows.Controls;

namespace SnowmobileWPF
{
    public partial class MainWindow : Window
    {
        private SearchWindow searchWindow;
        public MainWindow()
        {
            InitializeComponent();
            SourceComboBox.ItemsSource = Enum.GetValues(typeof(SubscriptionSource));
        }

        /// <summary>
        /// Input masking logic to ensure only numeric data is entered into specific fields.
        /// Prevents invalid data types from ever reaching the ViewModel.
        /// </summary>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // checks if searchWindow is already opened
            if (searchWindow == null || !searchWindow.IsLoaded)
            {
                searchWindow = new SearchWindow
                {
                    Owner = this
                };
            }
            if (searchWindow.WindowState == WindowState.Minimized)
            {
                searchWindow.WindowState = WindowState.Normal;
            }
            searchWindow.Show();
        }

        private void SubscriberList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedSubscriber != null)
            {
                vm.UpdateCommand.Execute(null);
            }
        }

        /// <summary>
        /// Focus management logic. 
        /// Uses Dispatcher.BeginInvoke to ensure the UI has finished rendering the 
        /// Edit mode before attempting to set focus and move the caret to the end of the text.
        /// </summary>
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

        /// <summary>
        /// Post-login notification logic.
        /// Triggers a non-intrusive ToolTip if the contest state changed while the user was away.
        /// </summary>
        private void ContestButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.CheckAcknowledged)
                {
                    ToolTip contestToolTip = new ToolTip
                    {
                        Content = "A contest has ended since last login.",
                        Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                        PlacementTarget = ContestButton,
                        StaysOpen = false
                    };
                    contestToolTip.IsOpen = true;
                }
            }
        }
    }
}