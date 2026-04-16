using System.Windows;
using SnowmobileWPF.ViewModels;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Handles the final UI-level validation and user confirmation.
        /// This method acts as a gatekeeper, checking for soft-errors (duplicates) 
        /// and hard-errors (validation rules) before committing changes to the database.
        /// </summary>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UpdateViewModel vm)
            {
                // Force validation of all UI fields
                vm.ValidateAllProperties();

                if (!vm.HasErrors)
                {
                    // Check for duplicate names in the database
                    if (vm.CheckForDuplicate())
                    {
                        var result = MessageBox.Show(
                            $"A subscriber named {vm.FirstName} {vm.LastName} already exists. Do you want to save this anyway?",
                            "Duplicate Detected",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    try
                    {
                        vm.SaveChanges();
                        DialogResult = true;
                    }
                    catch (ValidationException ex)
                    {
                        MessageBox.Show(
                            $"A data integrity error occurred:\n\n{ex.Message}",
                            "Critical Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
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