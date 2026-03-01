using SnowmobileLibrary.Enums;
using SnowmobileLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Subscription
{
    [Key]
    public int SubscriptionId { get; set; }

    [ForeignKey(nameof(Subscriber))]
    public int VSCA { get; set; }

    public Subscriber Subscriber { get; set; } = null!;

    [Required(ErrorMessage = "Expiration date is required.")]
    public DateOnly ExpDate { get; set; }

    [Required(ErrorMessage = "Renewal date is required.")]
    public DateOnly DateRenewed { get; set; }

    [Required]
    [Range(0, 4, ErrorMessage = "Issues remaining must be between 0 and 4.")]
    public int IssuesRemaining { get; set; }

    public SubscriptionSource? Source { get; set; }
}