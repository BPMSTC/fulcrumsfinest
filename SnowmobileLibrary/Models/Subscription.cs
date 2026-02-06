using SnowmobileLibrary.Enums;

namespace SnowmobileLibrary.Models
{
    public class Subscription
    {
        // Primary key
        public int SubscriptionId { get; set; }

        // Foreign key to Subscriber
        public int VSCA { get; set; }

        // Lifecycle
        public DateTime ExpDate { get; set; }
        public DateTime DateRenewed { get; set; }

        // Fulfillment & Tracking
        public int IssuesRemaining { get; set; }
        public SubscriptionSource Source { get; set; }
    }
}