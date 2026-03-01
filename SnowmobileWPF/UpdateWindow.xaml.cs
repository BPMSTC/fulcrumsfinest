using System.Windows;
using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF
{
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
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