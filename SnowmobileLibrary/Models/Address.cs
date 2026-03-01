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

        public Subscriber Subscriber { get; set; } = null!;

        [Required(ErrorMessage = "Street address is required.")]
        [MaxLength(100)]
        // Allows letters, numbers, spaces, and common address symbols (. , #)
        [RegularExpression(@"^[\p{L}\d\s\.\,\#\-\/]+$", ErrorMessage = "Street address contains invalid characters.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "City name contains invalid characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State/Province is required.")]
        [MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "State/Province contains invalid characters.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required.")]
        [MaxLength(20)]
        // Alphanumeric, spaces, and hyphens for international codes
        [RegularExpression(@"^[\p{L}\d\s\-]+$", ErrorMessage = "Invalid postal code format.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s\-\']+$", ErrorMessage = "Country contains invalid characters.")]
        public string Country { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }
    }
}