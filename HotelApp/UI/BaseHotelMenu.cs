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

            var roomView = _hotel.Rooms
                .Select(room => new
                {
                    room.Number,
                    room.Price,
                    Status = room.IsOccupied ? "Зайнято" : "Вільно"
                })
                .OrderBy(room => room.Number)
                .ToList();

            foreach (var r in roomView)
            {
                _logger.Print($"Кімната {r.Number} | Ціна: {r.Price} | {r.Status}");
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
