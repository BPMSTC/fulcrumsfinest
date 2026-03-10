using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SnowmobileWPF.ViewModels
{
    // ViewModel used for editing an existing Subscriber.
    // Inherits ObservableValidator to provide property change notification
    // and automatic DataAnnotations validation support for WPF bindings.
    public partial class UpdateViewModel : ObservableValidator
    {
        private readonly ILogger<UpdateViewModel> _logger;

        // Stores the original name so the UI header does not change
        // if the user edits FirstName/LastName while the screen is open.
        private readonly string _originalSubscriberName;

        // Underlying model being edited.
        public Subscriber Subscriber { get; }

        // UI header displayed on the edit screen.
        public string DisplayHeader => $"Editing {_originalSubscriberName}";

        public UpdateViewModel(Subscriber subscriber, ILogger<UpdateViewModel> logger)
        {
            _logger = logger;
            Subscriber = subscriber;

            if (string.IsNullOrWhiteSpace(subscriber.FirstName) && string.IsNullOrWhiteSpace(subscriber.LastName))
            {
                Subscriber.Active = true;
            }

            // Capture name once so the header stays stable during editing.
            _originalSubscriberName = $"{subscriber.FirstName} {subscriber.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(_originalSubscriberName))
                _originalSubscriberName = "New Subscriber";

            // Trace which subscriber instance is being edited.
            _logger.LogInformation("UpdateViewModel initialized for VSCA: {VSCA}", Subscriber.VSCA);
        }

        #region Wrapper Properties

        // Wrapper properties expose model values to the UI while allowing
        // DataAnnotations validation and change notification through SetProperty.

        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name is too short.")]
        [MaxLength(50, ErrorMessage = "First name is too long (maximum 50 characters).")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "First name contains invalid characters.")]
        public string FirstName
        {
            get => Subscriber.FirstName;

            // SetProperty updates the underlying model property, raises
            // PropertyChanged, and triggers validation because validate=true.
            set { SetProperty(Subscriber.FirstName, value, Subscriber, (u, n) => u.FirstName = n, true); }
        }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "Last name contains invalid characters.")]
        public string LastName
        {
            get => Subscriber.LastName;
            set { SetProperty(Subscriber.LastName, value, Subscriber, (u, n) => u.LastName = n, true); }
        }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^$|^[\+\d\s\.\(\)\-]+$", ErrorMessage = "Phone number contains invalid characters.")]
        [MaxLength(20, ErrorMessage = "Phone number is too long.")]
        public string Phone
        {
            // Null-safe access for UI binding.
            get => Subscriber.Phone ?? string.Empty;
            set { SetProperty(Subscriber.Phone, value, Subscriber, (u, n) => u.Phone = n, true); }
        }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(320)]
        [RegularExpression(@"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must follow format: user@domain.com")]
        public string Email
        {
            get => Subscriber.Email ?? string.Empty;
            set { SetProperty(Subscriber.Email, value, Subscriber, (u, n) => u.Email = n, true); }
        }

        [Required(ErrorMessage = "Street address is required.")]
        [MinLength(5, ErrorMessage = "Please enter a full street address (minimum 5 characters).")]
        [MaxLength(100, ErrorMessage = "Street address is too long (maximum 100 characters).")]
        [RegularExpression(@"^[\p{L}\d\s\.\,\#\-\/]+$", ErrorMessage = "Street address contains invalid characters.")]
        public string Street
        {
            get => Subscriber.Address.Street;

            // Note: This property writes to a nested object (Subscriber.Address).
            set { SetProperty(Subscriber.Address.Street, value, Subscriber.Address, (u, n) => u.Street = n, true); }
        }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City name is too short.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "City name contains invalid characters.")]
        public string City
        {
            get => Subscriber.Address.City;
            set { SetProperty(Subscriber.Address.City, value, Subscriber.Address, (u, n) => u.City = n, true); }
        }

        [Required(ErrorMessage = "State/Region is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State/Region is too short.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "State/Region contains invalid characters.")]
        public string Region
        {
            get => Subscriber.Address.Region;
            set { SetProperty(Subscriber.Address.Region, value, Subscriber.Address, (u, n) => u.Region = n, true); }
        }

        [Required(ErrorMessage = "Postal code is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Postal code must be between 3 and 20 characters.")]
        [RegularExpression(@"^(?=.*\d)[\p{L}\d\s\-]+$", ErrorMessage = "Postal code must include at least one number.")]
        public string PostalCode
        {
            get => Subscriber.Address.PostalCode;
            set { SetProperty(Subscriber.Address.PostalCode, value, Subscriber.Address, (u, n) => u.PostalCode = n, true); }
        }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country name is too short.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "Country contains invalid characters.")]
        public string Country
        {
            get => Subscriber.Address.Country;
            set { SetProperty(Subscriber.Address.Country, value, Subscriber.Address, (u, n) => u.Country = n, true); }
        }

        [Required]
        [Range(0, 4, ErrorMessage = "Issues remaining must be between 0 and 4.")]
        public int IssuesRemaining
        {
            get => Subscriber.Subscription.IssuesRemaining;
            set { SetProperty(Subscriber.Subscription.IssuesRemaining, value, Subscriber.Subscription, (u, n) => u.IssuesRemaining = n, true); }
        }

        #endregion

        // Forces validation across all bound properties.
        public void ValidateAllProperties()
        {
            base.ValidateAllProperties();
            OnPropertyChanged(nameof(HasErrors));
        }

        public void SaveChanges()
        {
            _logger.LogInformation("Preparing final save for VSCA: {VSCA}", Subscriber.VSCA);

            // Final normalization step to remove leading/trailing whitespace
            // before persistence.
            Subscriber.FirstName = FirstName?.Trim() ?? string.Empty;
            Subscriber.LastName = LastName?.Trim() ?? string.Empty;
            Subscriber.Phone = Phone?.Trim() ?? string.Empty;
            Subscriber.Email = Email?.Trim() ?? string.Empty;
            Subscriber.Address.Street = Street?.Trim() ?? string.Empty;
            Subscriber.Address.City = City?.Trim() ?? string.Empty;
            Subscriber.Address.Region = Region?.Trim() ?? string.Empty;
            Subscriber.Address.PostalCode = PostalCode?.Trim() ?? string.Empty;
            Subscriber.Address.Country = Country?.Trim() ?? string.Empty;

            // Safety-net validation: re-run full DataAnnotations validation against the
            // entire Subscriber model before saving. This ensures that even if UI-level
            // validation was bypassed or missed, the model still satisfies all validation rules.
            var context = new ValidationContext(Subscriber);
            var results = new List<ValidationResult>();

            // TryValidateObject checks every property decorated with DataAnnotations
            // and populates the results list with any validation failures.
            if (!Validator.TryValidateObject(Subscriber, context, results, true))
            {
                // If validation fails, log the first error and stop the save operation.
                var error = results.First().ErrorMessage;
                _logger.LogError("Hard validation failed on Save: {Error}", error);

                throw new ValidationException(error);
            }

            // At this point the model is fully validated and ready for the repository layer.
            _logger.LogInformation("Changes validated and ready for persistence for VSCA: {VSCA}", Subscriber.VSCA);
        }
    }
}