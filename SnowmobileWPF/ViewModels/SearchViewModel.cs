using Microsoft.Extensions.Logging;
using SnowmobileWPF.Models;

namespace SnowmobileWPF.ViewModels
{
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

        public string? LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string? FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string? VSCAText
        {
            get => _vscaText;
            set => SetProperty(ref _vscaText, value);
        }

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
                parameters.LastName ?? "Any", parameters.FirstName ?? "Any", parameters.VSCA?.ToString() ?? "Any");

            return parameters;
        }
    }
}