using Microsoft.Extensions.Logging;
using SnowmobileLibrary.Models;

namespace SnowmobileWPF.ViewModels
{
    public class UpdateViewModel : ViewModelBase
    {
        private readonly ILogger<UpdateViewModel> _logger;

        public Subscriber Subscriber { get; }
        public string DisplayHeader => $"Editing {Subscriber.FirstName} {Subscriber.LastName}";

        // We inject the logger along with the subscriber
        public UpdateViewModel(Subscriber subscriber, ILogger<UpdateViewModel> logger)
        {
            _logger = logger;
            Subscriber = subscriber;
            _logger.LogInformation("UpdateViewModel initialized for Subscriber VSCA: {VSCA}", Subscriber.VSCA);
        }

        public void SaveChanges()
        {
            _logger.LogInformation("Preparing to save changes for VSCA: {VSCA}", Subscriber.VSCA);

            if (Subscriber != null)
            {
                // Trimming strings
                Subscriber.FirstName = Subscriber.FirstName?.Trim();
                Subscriber.LastName = Subscriber.LastName?.Trim();
                Subscriber.Phone = Subscriber.Phone?.Trim();

                if (Subscriber.Address != null)
                {
                    Subscriber.Address.Street = Subscriber.Address.Street?.Trim();
                    Subscriber.Address.City = Subscriber.Address.City?.Trim();
                    Subscriber.Address.Region = Subscriber.Address.Region?.Trim();
                    Subscriber.Address.Country = Subscriber.Address.Country?.Trim();
                    Subscriber.Address.PostalCode = Subscriber.Address.PostalCode?.Trim();
                }

                _logger.LogDebug("Data cleanup/trimming completed for VSCA: {VSCA}", Subscriber.VSCA);
            }
            else
            {
                _logger.LogWarning("SaveChanges was called, but the Subscriber object was null.");
            }
        }
    }
}