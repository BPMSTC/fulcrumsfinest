using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnowmobileLibrary.Models
{
    public class Email
    {
        [Key]
        public int EmailId { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public int VSCA { get; set; }

        public Subscriber Subscriber { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(320)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } // Whether or not the email is active
    }
}