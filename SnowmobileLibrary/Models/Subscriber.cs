using System.ComponentModel.DataAnnotations;

namespace SnowmobileLibrary.Models
{
    public class Subscriber
    {
        [Key]
        public int VSCA { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
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
    }
}