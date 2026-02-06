namespace SnowmobileLibrary.Models
{
    public class Subscriber
    {
        // Primary key / identifier for subscriber
        public int VSCA { get; set; }

        // Identity
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Contact
        public string Phone { get; set; }
        public string Email { get; set; }

        // Flags
        public bool Active { get; set; }
        public bool Contest { get; set; }
        public bool ManualMail { get; set; }

        // Advertiser copy, not a paid subscriber
        public bool Commercial { get; set; }

        // Metadata
        public DateTime DateJoined { get; set; }
        public string? Notes { get; set; }
    }
}