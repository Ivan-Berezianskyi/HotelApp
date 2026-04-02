using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.UI
{
    internal abstract class BaseHotelMenu : BaseMenu
    {
        protected Hotel _hotel;

        public BaseHotelMenu(Hotel hotel, ILogger logger) : base(logger)
        { 
            _hotel = hotel; 
        }

        protected void ShowRooms()
        {
            _logger.Print("\n--- ПОВНИЙ СПИСОК НОМЕРІВ ---");
            if (_hotel.Rooms.Count == 0)
            {
                _logger.Print("Список порожній.");
                return;
            }

            foreach (var r in _hotel.Rooms)
            {
                _logger.Print($"Кімната {r.Number} | Ціна: {r.Price} | {(r.IsOccupied ? "Зайнято" : "Вільно")}");
            }
        }

        protected bool LogIfError(string? errorMessage)
        {
            if (errorMessage != null) 
            {
                _logger.Print(errorMessage);
                return true;
            }

            return false;
        }
    }
}
