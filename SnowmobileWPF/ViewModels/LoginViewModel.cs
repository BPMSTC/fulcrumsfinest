using SnowmobileWPF.ViewModels;

namespace SnowmobileWPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _serverIp = "127.0.0.1:1433";
        private string _username = string.Empty;

        public string ServerIp
        {
            get => _serverIp;
            set => SetProperty(ref _serverIp, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public bool Authenticate(string password)
        {
            // eventual call to Library/Database logic (all placeholder)
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
                return false;

            return true;
        }
    }
}