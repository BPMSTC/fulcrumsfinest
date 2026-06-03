namespace SnowmobileLibrary.Services
{
    public class FileLogger : ILogger
    {
        private readonly string _logDirectory;
        private readonly string _logPath;
        private const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2MB Limit

        public FileLogger()
        {
            // Stores logs in: C:\Users\<User>\AppData\Local\VSCASubscriberManager\
            _logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VSCASubscriberManager");

            _logPath = Path.Combine(_logDirectory, "application_log.txt");

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }


        // Logging
        public void LogInfo(string message) => WriteEntry("INFO", message);
        public void LogWarning(string message) => WriteEntry("WARN", message);
        public void LogError(string message, Exception? ex = null)
        {
            string fullMessage = ex != null
                ? $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}"
                : message;
            WriteEntry("ERROR", fullMessage);
        }


        // Private Helpers
        private void WriteEntry(string level, string message)
        {
            try
            {
                RotateLogIfLarge();

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string entry = $"{timestamp} [{level.PadRight(5)}] {message}{Environment.NewLine}";

                File.AppendAllText(_logPath, entry);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Critical Logger Failure: {ex.Message}");
            }
        }

        private void RotateLogIfLarge()
        {
            if (File.Exists(_logPath) && new FileInfo(_logPath).Length > MaxFileSizeBytes)
            {
                string archivePath = Path.Combine(_logDirectory, $"log_archive_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                File.Move(_logPath, archivePath);
            }
        }
    }
}