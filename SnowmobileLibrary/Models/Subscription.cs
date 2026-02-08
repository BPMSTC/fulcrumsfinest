using SnowmobileLibrary.Enums;

namespace SnowmobileLibrary.Models
{
    public class Subscription
    {
        // Primary key
        public int SubscriptionId { get; set; }

        // Foreign key to Subscriber
        public Subscriber VSCA { get; set; }

        // Lifecycle
        public DateOnly ExpDate { get; set; }
        public DateOnly DateRenewed { get; set; }

        // Fulfillment & Tracking
        public int IssuesRemaining { get; set; }
        public SubscriptionSource Source { get; set; }
    }
}