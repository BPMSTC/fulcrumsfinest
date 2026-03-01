using System.ComponentModel.DataAnnotations;

namespace SnowmobileLibrary.Models
{
    public class Subscriber
    {
        [Key]
        public int VSCA { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50)]
        // allow spaces, hyphens, and apostrophes.
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "First name contains invalid characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "Last name contains invalid characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        // International phones often start with + and can contain spaces/dots
        [RegularExpression(@"^[\+\d\s\.\(\)\-]+$", ErrorMessage = "Phone number contains invalid characters.")]
        public string Phone { get; set; } = string.Empty;

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

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        public Subscription Subscription { get; set; }

        public Email? Email { get; set; }

        public override string ToString() => $"{FirstName} {LastName} (VSCA: {VSCA})";
    }
}