using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SnowmobileLibrary.Services
{
    /// <summary>
    /// Integrates the custom FileLogger into the standard .NET ILogging infrastructure.
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly FileLogger _fileLogger = new();
        // Caches loggers by category (e.g. Namespace.ClassName) to avoid redundant allocations
        private readonly ConcurrentDictionary<string, MicrosoftFileLogger> _loggers = new();

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new MicrosoftFileLogger(_fileLogger, name));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        /// <summary>
        /// A wrapper class that maps Microsoft's LogLevel and formatting to the custom FileLogger.
        /// </summary>
        private class MicrosoftFileLogger : Microsoft.Extensions.Logging.ILogger
        {
            private readonly FileLogger _fileLogger;
            private readonly string _category;

            public MicrosoftFileLogger(FileLogger fileLogger, string category)
            {
                _fileLogger = fileLogger;
                _category = category;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                // Uses the built-in formatter to handle template strings (e.g. "Hello {Name}")
                var message = formatter(state, exception);
                var fullEntry = $"[{_category}] {message}";

                // Maps standard Microsoft log levels to our custom tri-level logger
                if (logLevel >= LogLevel.Error)
                    _fileLogger.LogError(fullEntry, exception);
                else if (logLevel == LogLevel.Warning)
                    _fileLogger.LogWarning(fullEntry);
                else
                    _fileLogger.LogInfo(fullEntry);
            }
        }
    }
}