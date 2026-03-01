using SnowmobileLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Email
{
    [Key]
    public int EmailId { get; set; }

    [ForeignKey(nameof(Subscriber))]
    public int VSCA { get; set; }

    public Subscriber Subscriber { get; set; } = null!;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(320)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must follow format: user@domain.com")]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; }
}