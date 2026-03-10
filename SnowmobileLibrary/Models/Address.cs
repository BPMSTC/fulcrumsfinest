using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnowmobileLibrary.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public int VSCA { get; set; }

        public Subscriber SubscriberObject { get; set; } = null!;

        [Required(ErrorMessage = "Street address is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Street address must be between 5 and 100 characters.")]
        [RegularExpression(@"^[\p{L}\d\s\.\,\#\-\/]+$", ErrorMessage = "Street address contains invalid characters.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City name is too short.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "City name contains invalid characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State/Province is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State/Province is too short.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "State/Province contains invalid characters.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Postal code must be between 3 and 20 characters.")]
        // Mandates at least one number, allows letters, spaces, and hyphens.
        [RegularExpression(@"^(?=.*\d)[\p{L}\d\s\-]+$", ErrorMessage = "Postal code must include at least one number.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country name is too short.")]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "Country contains invalid characters.")]
        public string Country { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }
    }
}