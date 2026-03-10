using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;
using SnowmobileWPF.Models;
using SnowmobileWPF.Repositories;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SnowmobileWPF.ViewModels
{
    public partial class UpdateViewModel : ObservableValidator
    {
        private readonly ILogger<UpdateViewModel> _logger;
        private readonly ISubscriberRepository _repository;
        private readonly string _originalSubscriberName;

        public Subscriber Subscriber { get; }
        public string DisplayHeader => $"Editing {_originalSubscriberName}";

        public UpdateViewModel(Subscriber subscriber, ISubscriberRepository repository, ILogger<UpdateViewModel> logger)
        {
            _logger = logger;
            _repository = repository;
            Subscriber = subscriber;

            if (string.IsNullOrWhiteSpace(subscriber.FirstName) && string.IsNullOrWhiteSpace(subscriber.LastName))
            {
                Subscriber.Active = true;
            }

            _originalSubscriberName = $"{subscriber.FirstName} {subscriber.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(_originalSubscriberName))
                _originalSubscriberName = "New Subscriber";

            _logger.LogInformation("UpdateViewModel initialized for VSCA: {VSCA}", Subscriber.VSCA);
        }

        #region Wrapper Properties

        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name is too short.")]
        [MaxLength(50, ErrorMessage = "First name is too long.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "First name contains invalid characters.")]
        public string FirstName
        {
            get => Subscriber.FirstName;
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

        [RegularExpression(@"^$|^[\+\d\s\.\(\)\-]+$", ErrorMessage = "Phone number contains invalid characters.")]
        [MaxLength(20, ErrorMessage = "Phone number is too long.")]
        public string Phone
        {
            get => Subscriber.Phone ?? string.Empty;
            set
            {
                var val = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                SetProperty(Subscriber.Phone, val, Subscriber, (u, n) => u.Phone = n, true);
            }
        }

        [MaxLength(320)]
        [RegularExpression(@"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must follow format: user@domain.com")]
        public string Email
        {
            get => Subscriber.Email ?? string.Empty;
            set
            {
                var val = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                SetProperty(Subscriber.Email, val, Subscriber, (u, n) => u.Email = n, true);
            }
        }

        [Required(ErrorMessage = "Street address is required.")]
        [MinLength(5, ErrorMessage = "Street address too short.")]
        [MaxLength(100, ErrorMessage = "Street address too long.")]
        [RegularExpression(@"^[\p{L}\d\s\.\,\#\-\/]+$", ErrorMessage = "Street address contains invalid characters.")]
        public string Street
        {
            get => Subscriber.Address.Street;
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

        public void ValidateAllProperties()
        {
            base.ValidateAllProperties();
            OnPropertyChanged(nameof(HasErrors));
        }

        // Checks if a subscriber with the same name already exists in the database.
        public bool CheckForDuplicate()
        {
            var results = _repository.Search(new SearchParams
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim()
            });

            // If we are editing (VSCA > 0), we exclude the current record from duplicate matches
            return results != null && results.Any(s => s.VSCA != Subscriber.VSCA);
        }

        public void SaveChanges()
        {
            _logger.LogInformation("Preparing final save for VSCA: {VSCA}", Subscriber.VSCA);

            Subscriber.FirstName = FirstName?.Trim() ?? string.Empty;
            Subscriber.LastName = LastName?.Trim() ?? string.Empty;
            Subscriber.Phone = Phone?.Trim() ?? string.Empty;
            Subscriber.Email = Email?.Trim() ?? string.Empty;
            Subscriber.Address.Street = Street?.Trim() ?? string.Empty;
            Subscriber.Address.City = City?.Trim() ?? string.Empty;
            Subscriber.Address.Region = Region?.Trim() ?? string.Empty;
            Subscriber.Address.PostalCode = PostalCode?.Trim() ?? string.Empty;
            Subscriber.Address.Country = Country?.Trim() ?? string.Empty;

            var context = new ValidationContext(Subscriber);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(Subscriber, context, results, true))
            {
                var error = results.First().ErrorMessage;
                _logger.LogError("Hard validation failed on Save: {Error}", error);
                throw new ValidationException(error);
            }

            _logger.LogInformation("Changes validated for VSCA: {VSCA}", Subscriber.VSCA);
        }
    }
}