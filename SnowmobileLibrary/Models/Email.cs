namespace SnowmobileLibrary.Models
{
    public class Email
    {
        // Primary key
        public int EmailId { get; set; }

        // Foreign key to Subscriber
        public Subscriber VSCA { get; set; }

        // Email address
        public string EmailAddress { get; set; }
    }
}