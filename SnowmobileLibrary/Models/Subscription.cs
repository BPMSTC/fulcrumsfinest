using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SnowmobileLibrary.Enums;

namespace SnowmobileLibrary.Models
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public int VSCA { get; set; }

        public Subscriber Subscriber { get; set; } = null!;

        [Required]
        public DateOnly ExpDate { get; set; } // Subscription expiration date

        [Required]
        public DateOnly DateRenewed { get; set; } // Date the subscription was last renewed

        [Range(0, 4, ErrorMessage = "Issues remaining must be between 0 and 4.")]
        public int IssuesRemaining { get; set; }

        [Required]
        public SubscriptionSource Source { get; set; } // Source of subscription
    }
}