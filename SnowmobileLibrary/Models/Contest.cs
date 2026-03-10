using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SnowmobileLibrary.Models
{
    public class Contest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndDate { get; set; }

        public bool Acknowledged { get; set; }
    }
}
