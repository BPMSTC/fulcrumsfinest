using SnowmobileLibrary.Models;
using SnowmobileWPF.ViewModels;
using System.Windows;

namespace SnowmobileWPF
{
    public partial class RenewWindow : Window
    {
        public RenewWindow(RenewViewModel vm, Subscriber subscriber)
        {
            InitializeComponent();
            vm.CurrentSubscriber = subscriber;
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
