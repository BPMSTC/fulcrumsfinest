using System.ComponentModel.DataAnnotations;

namespace SnowmobileLibrary.Models
{
    public class Subscriber
    {
        [Key]
        public int VSCA { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "First name contains invalid characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "Last name contains invalid characters.")]
        public string LastName { get; set; } = string.Empty;

        [RegularExpression(@"^$|^[\+\d\s\.\(\)\-]+$", ErrorMessage = "Phone number contains invalid characters.")]
        [StringLength(20, ErrorMessage = "Phone number is too long.")]
        public string? Phone { get; set; } = string.Empty;

        [Required]
        public bool Active { get; set; }

        [Required]
        public bool Contest { get; set; }

        [Required]
        public bool ManualMail { get; set; }

        [Required]
        public bool Commercial { get; set; }

        [Required]
        public DateOnly DateJoined { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public Subscription Subscription { get; set; }

        [MaxLength(320)]
        [RegularExpression(@"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must follow format: user@domain.com")]
        public string? Email { get; set; } = string.Empty;

        public override string ToString() => $"{FirstName} {LastName} (VSCA: {VSCA})";
        public string PhoneDisplay => string.IsNullOrWhiteSpace(Phone) ? "No Phone Number Provided" : Phone;
        public string EmailDisplay => string.IsNullOrWhiteSpace(Email) ? "No Email Provided" : Email;
    }
}