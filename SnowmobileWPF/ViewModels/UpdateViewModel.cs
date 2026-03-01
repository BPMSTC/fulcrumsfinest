using SnowmobileLibrary.Models;

namespace SnowmobileWPF.ViewModels
{
    public class UpdateViewModel : ViewModelBase
    {
        // Reference to the subscriber being edited
        public Subscriber Subscriber { get; }

        // Dynamic header for the window
        public string DisplayHeader => $"Editing {Subscriber}";

        public UpdateViewModel(Subscriber subscriber)
        {
            Subscriber = subscriber;
        }

        public void SaveChanges()
        {
            if (Subscriber != null)
            {
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
            }
        }
    }
}