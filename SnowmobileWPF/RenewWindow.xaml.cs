using SnowmobileLibrary.Models;
using SnowmobileWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
