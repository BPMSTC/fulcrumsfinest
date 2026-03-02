using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace SnowmobileWPF.ViewModels
{
    public class UpdateViewModel : ViewModelBase
    {
        private readonly ILogger<UpdateViewModel> _logger;
        private readonly string _originalSubscriberName;

        public Subscriber Subscriber { get; }

        public string DisplayHeader => $"Editing {_originalSubscriberName}";

        public UpdateViewModel(Subscriber subscriber, ILogger<UpdateViewModel> logger)
        {
            _logger = logger;
            Subscriber = subscriber;

            // Capture the initial name for the header so it remains static during edits
            _originalSubscriberName = $"{subscriber.FirstName} {subscriber.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(_originalSubscriberName))
                _originalSubscriberName = "New Subscriber";

            _logger.LogInformation("UpdateViewModel initialized for VSCA: {VSCA}", Subscriber.VSCA);
        }

        #region Wrapper Properties

        public string FirstName
        {
            get => Subscriber.FirstName;
            set { Subscriber.FirstName = value; OnPropertyChanged(); }
        }

        public string LastName
        {
            get => Subscriber.LastName;
            set { Subscriber.LastName = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => Subscriber.Phone;
            set { Subscriber.Phone = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => Subscriber.Email ?? string.Empty;
            set
            {
                if (Subscriber.Email != null)
                {
                    Subscriber.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Street
        {
            get => Subscriber.Address.Street;
            set { Subscriber.Address.Street = value; OnPropertyChanged(); }
        }

        public string City
        {
            get => Subscriber.Address.City;
            set { Subscriber.Address.City = value; OnPropertyChanged(); }
        }

        public string Region
        {
            get => Subscriber.Address.Region;
            set { Subscriber.Address.Region = value; OnPropertyChanged(); }
        }

        public string PostalCode
        {
            get => Subscriber.Address.PostalCode;
            set { Subscriber.Address.PostalCode = value; OnPropertyChanged(); }
        }

        public string Country
        {
            get => Subscriber.Address.Country;
            set { Subscriber.Address.Country = value; OnPropertyChanged(); }
        }

        public int IssuesRemaining
        {
            get => Subscriber.Subscription.IssuesRemaining;
            set { Subscriber.Subscription.IssuesRemaining = value; OnPropertyChanged(); }
        }
        #endregion

        #region Validation Logic
        public override string this[string columnName]
        {
            get
            {
                object target = Subscriber;
                string modelPropertyName = columnName;

                // ROUTING: Redirect validation to the specific sub-model
                if (new[] { "Street", "City", "Region", "PostalCode", "Country" }.Contains(columnName))
                {
                    target = Subscriber.Address;
                }
                else if (columnName == nameof(IssuesRemaining))
                {
                    target = Subscriber.Subscription;
                }
                //else if (columnName == nameof(EmailAddress))
                //{
                //    target = Subscriber.Email;
                //    modelPropertyName = "EmailAddress";
                //}

                if (target == null) return string.Empty;

                // Execute Data Annotation validation
                var results = new List<ValidationResult>();
                var context = new ValidationContext(target) { MemberName = modelPropertyName };
                var propInfo = target.GetType().GetProperty(modelPropertyName);

                if (propInfo == null) return string.Empty;

                var value = propInfo.GetValue(target);

                if (!Validator.TryValidateProperty(value, context, results))
                {
                    return results.First().ErrorMessage ?? "Invalid value";
                }

                return string.Empty;
            }
        }
        #endregion

        public void SaveChanges()
        {
            _logger.LogInformation("Preparing to save changes for VSCA: {VSCA}", Subscriber.VSCA);

            // Data Sanitization
            FirstName = FirstName?.Trim() ?? string.Empty;
            LastName = LastName?.Trim() ?? string.Empty;
            Phone = Phone?.Trim() ?? string.Empty;
            Email = Email?.Trim() ?? string.Empty;
            Street = Street?.Trim() ?? string.Empty;
            City = City?.Trim() ?? string.Empty;
            Region = Region?.Trim() ?? string.Empty;
            PostalCode = PostalCode?.Trim() ?? string.Empty;
            Country = Country?.Trim() ?? string.Empty;

            _logger.LogDebug("Data cleanup/trimming completed for VSCA: {VSCA}", Subscriber.VSCA);
        }
    }
}