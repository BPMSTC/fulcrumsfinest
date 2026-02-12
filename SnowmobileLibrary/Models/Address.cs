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

        [Required]
        [MaxLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Region { get; set; } = string.Empty; // State or Province

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty; // ZIP or postal

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }
    }
}