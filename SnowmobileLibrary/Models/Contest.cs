using System.ComponentModel.DataAnnotations;

namespace SnowmobileLibrary.Models
{
    public class Contest
    {
        // Id
        [Key]
        public int Id { get; set; }


        // Start & End Date
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndDate { get; set; }


        // Acknowledged
        public bool Acknowledged { get; set; }
    }
}