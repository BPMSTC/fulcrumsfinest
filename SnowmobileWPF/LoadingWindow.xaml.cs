using System.Windows;
using System.Windows.Controls;

namespace SnowmobileWPF
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        /// <summary>
        /// Provides a thread-safe progress reporter.
        /// Bridges the background import task to the UI thread to update the ProgressBar and status label.
        /// </summary>
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