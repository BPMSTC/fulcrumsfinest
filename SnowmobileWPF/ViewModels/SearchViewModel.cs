using Microsoft.Extensions.Logging;
using SnowmobileWPF.Models;

namespace SnowmobileWPF.ViewModels
{
    /// <summary>
    /// Coordinates the multi-field search interface.
    /// Manages the reactive "CanSearch" state to ensure the UI only allows execution when 
    /// actionable criteria have been provided.
    /// </summary>
    public class SearchViewModel : ViewModelBase
    {
        private readonly ILogger<SearchViewModel> _logger;
        private string? _lastName;
        private string? _firstName;
        private string? _phoneNumber;
        private string? _vscaText;

        public SearchViewModel(ILogger<SearchViewModel> logger)
        {
            _logger = logger;
        }

        // Logic to determine if the Search button should be enabled.
        // Returns true if at least one field contains non-whitespace text.
        public bool CanSearch =>
            !string.IsNullOrWhiteSpace(LastName) ||
            !string.IsNullOrWhiteSpace(FirstName) ||
            !string.IsNullOrWhiteSpace(PhoneNumber) ||
            !string.IsNullOrWhiteSpace(VSCAText);

        public string? LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value))
                    OnPropertyChanged(nameof(CanSearch));
            }
        }

        public string? FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value))
                    OnPropertyChanged(nameof(CanSearch));
            }
        }

        public string? PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (SetProperty(ref _phoneNumber, value))
                    OnPropertyChanged(nameof(CanSearch));
            }
        }

        public string? VSCAText
        {
            get => _vscaText;
            set
            {
                if (SetProperty(ref _vscaText, value))
                    OnPropertyChanged(nameof(CanSearch));
            }
        }

        /// <summary>
        /// Gathers the UI input and packages it into a SearchParams object.
        /// Converts empty strings to null for cleaner database queries and ensures VSCA 
        /// is handled as a numeric type.
        /// </summary>
        public SearchParams GetParameters()
        {
            _logger.LogDebug("Constructing SearchParams from UI input.");

            var parameters = new SearchParams
            {
                LastName = string.IsNullOrWhiteSpace(LastName) ? null : LastName.Trim(),
                FirstName = string.IsNullOrWhiteSpace(FirstName) ? null : FirstName.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber.Trim(),
                VSCA = int.TryParse(VSCAText, out int vsca) ? vsca : null
            };

            _logger.LogInformation("Search Parameters Built: Last={Last}, First={First}, VSCA={VSCA}",
                parameters.LastName ?? "Any",
                parameters.FirstName ?? "Any",
                parameters.VSCA?.ToString() ?? "Any");

            return parameters;
        }
    }
}