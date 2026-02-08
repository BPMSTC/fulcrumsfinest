namespace SnowmobileLibrary.Models
{
    public class Address
    {
        // Primary key
        public int AddressId { get; set; }

        // Foreign key to Subscriber
        public Subscriber VSCA { get; set; }

        public string Street { get; set; }
        public string City { get; set; }

        // State or province
        public string Region { get; set; }

        // ZIP or postal code
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}