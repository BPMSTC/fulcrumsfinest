namespace SnowmobileWPF.Models
{
    public class DbSettings
    {
        // stores connection string after receiving details from LoginWindow
        // defaults to localdb for development/testing purposes
        public string ConnectionString { get; set; } = string.Empty;
    }
}