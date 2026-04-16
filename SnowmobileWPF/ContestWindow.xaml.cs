using SnowmobileWPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SnowmobileWPF
{
    /// <summary>
    /// Interaction logic for ContestWindow.xaml
    /// </summary>
    public partial class ContestWindow : Window
    {
        public ContestWindow(ContestViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        /// <summary>
        /// Manual event trigger to bridge a mouse event on a TextBlock to a ViewModel command.
        /// Useful when the UI element does not natively support the Command property.
        /// </summary>
        private void StopTextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ContestViewModel vm)
            {
                if (vm.StopCommand.CanExecute(null))
                {
                    vm.StopCommand.Execute(null);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}