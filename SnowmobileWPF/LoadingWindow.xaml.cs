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
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public Progress<int> progress { get; set; }
        public LoadingWindow()
        {
            InitializeComponent();
            progress = new Progress<int>(value =>
            {
                ProgressBar.Value = value;
                VerboseProgress.Content = $"{value}% done";
            });
        }
    }
}
