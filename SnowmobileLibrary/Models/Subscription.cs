using SnowmobileLibrary.Enums;
using SnowmobileLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Subscription
{
    // Id & Foreign Key to Subscriber
    [Key]
    public int SubscriptionId { get; set; }

    [ForeignKey(nameof(Subscriber))]
    public int VSCA { get; set; }


    // Expiration Date & Renewal Date
    [Required(ErrorMessage = "Expiration date is required.")]
    public DateOnly ExpDate { get; set; }

    [Required(ErrorMessage = "Renewal date is required.")]
    public DateOnly DateRenewed { get; set; }


    // Issues Remaining & Subscription Source
    public SubscriptionSource? Source { get; set; }

    [Required]
    public int IssuesRemaining { get; set; }


    // Navigation Property
    public Subscriber Subscriber { get; set; } = null!;


    // Final Issue & Eligibility Logic
    public string FinalIssue 
    { 
        get
        {
            // default to December to simplify subscriptions ending before the March cutoff.
            string month = "December";
            string year = ExpDate.Year.ToString();

            // check if subscriber is eligible for next issue by passing the cutoff month
            if (IsEligible(2))
            {
                month = "March";
            }
            if (IsEligible(5))
            {
                month = "June";
            } 
            if (IsEligible(8))
            {
                month = "September";
            }
            if (IsEligible(11))
            {
                month = "December";
            }

            // handle expiration dates before the March cutoff, which would make the final issue the December of the previous year
            if (month == "December" && ExpDate <= new DateOnly(ExpDate.Year, 2, 10))
            {
                year = (ExpDate.Year - 1).ToString();
            }
            
            return $"{month} {year}";
        } 
    }

    private bool IsEligible(int month)
    {
        // cutoff date for all subscriptions is the 10th of the month before the issue
        DateOnly cutoff = new(ExpDate.Year, month, 10);

        if (ExpDate > cutoff)
        {
            return true;
        } 
        else
        {
            return false;
        }
    }
}