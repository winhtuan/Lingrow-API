using System.Runtime.CompilerServices;
using System.Text;

namespace Lingrow.BusinessLogicLayer.Helper
{
    public static class LoggerHelper
    {
        private static readonly object _lock = new object();
        private static string _logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

        public static void Configure(string contentRootPath)
        {
            if (!string.IsNullOrWhiteSpace(contentRootPath))
                _logDirectory = Path.Combine(contentRootPath, "Logs");

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            string logFileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
            string logFilePath = Path.Combine(_logDirectory, logFileName);
            if (File.Exists(logFilePath))
            {
                try
                {
                    File.Delete(logFilePath);
                }
                catch { }
            }
        }

        public static void Info(string message, [CallerFilePath] string callerFilePath = "") =>
            Log(LogLevel.Info, message, GetClassName(callerFilePath));

        public static void Warn(string message, [CallerFilePath] string callerFilePath = "") =>
            Log(LogLevel.Warn, message, GetClassName(callerFilePath));

        public static void Error(string message, [CallerFilePath] string callerFilePath = "") =>
            Log(LogLevel.Error, message, GetClassName(callerFilePath));

        public static void Error(Exception ex, [CallerFilePath] string callerFilePath = "")
        {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);

            var inner = ex.InnerException;
            while (inner != null)
            {
                sb.AppendLine("--- Inner Exception ---");
                sb.AppendLine(inner.Message);
                sb.AppendLine(inner.StackTrace);
                inner = inner.InnerException;
            }

            Log(LogLevel.Error, sb.ToString(), GetClassName(callerFilePath));
        }

        private static void Log(LogLevel level, string message, string className)
        {
            lock (_lock)
            {
                try
                {
                    string logFileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
                    string logFilePath = Path.Combine(_logDirectory, logFileName);

                    string line =
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{className}] [{level}] : {message}{Environment.NewLine}";

                    File.AppendAllText(logFilePath, line, Encoding.UTF8);
                }
                catch { }
            }
        }

        private static string GetClassName(string filePath) =>
            Path.GetFileNameWithoutExtension(filePath);

        private enum LogLevel
        {
            Info,
            Warn,
            Error,
        }
    }
}
