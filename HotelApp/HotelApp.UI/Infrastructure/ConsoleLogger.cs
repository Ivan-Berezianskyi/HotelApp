using HotelApp.Interfaces;

namespace HotelApp.Infrastructure
{
    internal class ConsoleLogger : ILogger
    {
        public void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
