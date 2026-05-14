using HotelApp.Interfaces;

namespace HotelApp.Infrastructure
{
    internal class LoggerComposite : ILogger
    {
        private readonly List<ILogger> _loggers = new List<ILogger>();

        public LoggerComposite()
        {
        }

        public void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        public void Print(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Print(message);
            }
        }
    }
}