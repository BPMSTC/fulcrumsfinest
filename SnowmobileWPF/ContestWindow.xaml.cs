using SnowmobileWPF.ViewModels;
using System.Windows;

namespace SnowmobileWPF
{
    public partial class ContestWindow : Window
    {
        public ContestWindow(ContestViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
