using System.Windows;
using SnowmobileWPF.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SnowmobileWPF
{
    internal enum UpdateMode
    {
        Create,
        Edit
    }

    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
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
                // Forces viewmodel to check all of the properties
                vm.ValidateAllProperties();

                // Check UI/Viewmodel layer status
                if (!vm.HasErrors)
                {
                    try
                    {
                        vm.SaveChanges();
                        DialogResult = true;
                    }
                    catch (ValidationException ex)
                    {
                        // This catches any data integrity issues that bypassed the UI
                        MessageBox.Show(
                            $"A data integrity error occurred:\n\n{ex.Message}",
                            "Critical Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Safety fallback if the user manages to click Save while the button is disabled
                    MessageBox.Show(
                        "Please correct the highlighted errors before saving.",
                        "Validation Errors",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }
    }
}