using System.Windows;
using SnowmobileWPF.ViewModels;
using System.Text;
using System.Collections.Generic;

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
                string[] propertiesToValidate =
                {
                    nameof(vm.FirstName),
                    nameof(vm.LastName),
                    nameof(vm.Phone),
                    //nameof(vm.Email),
                    nameof(vm.Street),
                    nameof(vm.City),
                    nameof(vm.Region),
                    nameof(vm.PostalCode),
                    nameof(vm.Country)
                };

                List<string> errorList = new List<string>();

                // Collect every error that exists
                foreach (string property in propertiesToValidate)
                {
                    string error = vm[property];
                    if (!string.IsNullOrEmpty(error))
                    {
                        errorList.Add(error);
                    }
                }

                // If any errors were found, display them all in one go
                if (errorList.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Please correct the following errors before saving:");
                    sb.AppendLine();

                    foreach (var error in errorList)
                    {
                        sb.AppendLine($"• {error}");
                    }

                    MessageBox.Show(
                        sb.ToString(),
                        "Validation Errors",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                // If we got here, no errors were found
                vm.SaveChanges();
                DialogResult = true;
            }
        }
    }
}