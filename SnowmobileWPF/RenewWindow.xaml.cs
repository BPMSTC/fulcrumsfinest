using SnowmobileLibrary.Models;
using SnowmobileWPF.ViewModels;
using System.Windows;

namespace SnowmobileWPF
{
    /// <summary>
    /// Interaction logic for RenewWindow.xaml
    /// </summary>
    public partial class RenewWindow : Window
    {
        public RenewWindow(RenewViewModel vm, Subscriber subscriber)
        {
            InitializeComponent();
            vm.CurrentSubscriber = subscriber;
            // The ViewModel triggers this Action, allowing the logic layer
            // to close the View without a direct reference to it.
            vm.CloseWindow = () => HandleClose();
            DataContext = vm;
        }

        private void HandleClose()
        {
            MessageBox.Show("Subscriber renewed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
        }
    }
}