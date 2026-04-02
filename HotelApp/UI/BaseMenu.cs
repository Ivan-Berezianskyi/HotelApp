using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.UI
{
    internal abstract class BaseMenu : IMenu
    {
        protected ILogger _logger;

        public BaseMenu(ILogger logger) 
        { 
            _logger = logger; 
        }

        public abstract void Display();

        protected bool TryReadPositiveNumber(out double num)
        {
            if (!double.TryParse(Console.ReadLine(), out num) || num <= 0)
            {
                _logger.Print("Помилка: число має бути додатним.");
                return false;
            }

            return true;
        }

        protected bool TryReadPositiveNumber(out int num)
        {
            if (!int.TryParse(Console.ReadLine(), out num) || num <= 0)
            {
                _logger.Print("Помилка: число має бути додатним цілим.");
                return false;
            }

            return true;
        }

        protected bool TryReadNonNegativeNumber(out int num)
        {
            if (!int.TryParse(Console.ReadLine(), out num) || num < 0)
            {
                _logger.Print("Помилка: число має бути невід'ємним цілим.");
                return false;
            }

            return true;
        }
    }
}
