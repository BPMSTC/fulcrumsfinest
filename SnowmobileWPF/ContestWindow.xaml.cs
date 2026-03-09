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
    /// Interaction logic for ContestWindow.xaml
    /// </summary>
    public partial class ContestWindow : Window
    {
        public ContestWindow(ContestViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

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
    }
}
