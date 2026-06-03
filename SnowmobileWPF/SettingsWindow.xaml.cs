using SnowmobileWPF.ViewModels;
using System.Windows;

namespace SnowmobileWPF
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel vm)
        {
            InitializeComponent();
            vm.CloseWindow = () => Close();
            DataContext = vm;
        }
    }
}
