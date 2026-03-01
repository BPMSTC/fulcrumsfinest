using System.Windows;
using SnowmobileLibrary.Models;
using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF
{
    public partial class UpdateWindow : Window
    {
        public UpdateWindow(Subscriber subscriber)
        {
            InitializeComponent();
            DataContext = new UpdateViewModel(subscriber);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UpdateViewModel vm)
            {
                vm.SaveChanges();
                DialogResult = true;
            }
        }
    }
}