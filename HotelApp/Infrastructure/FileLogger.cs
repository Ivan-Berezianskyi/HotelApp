using HotelApp.Interfaces;

namespace HotelApp.Infrastructure
{
    internal class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private readonly object _syncRoot = new object();

        public FileLogger(string logFilePath = "hotel.log")
        {
            _logFilePath = logFilePath;
        }

        public void Print(string message)
        {
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            lock (_syncRoot)
            {
                string? directory = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.AppendAllText(_logFilePath, line + Environment.NewLine);
            }
        }

    }
}
