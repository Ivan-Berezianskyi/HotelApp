using HotelApp.Interfaces;
using HotelApp.UI.Interfaces;
namespace HotelApp.UI
{
    internal abstract class BaseHotelMenu : BaseMenu
    {
        protected readonly UserSession _session;
        protected readonly IHotelApiClient _apiClient;

        public BaseHotelMenu(UserSession session, ILogger logger, IHotelApiClient apiClient) : base(logger)
        {
            _session = session;
            _apiClient = apiClient;
        }

        protected void ShowRooms()
        {
            _logger.Print("\n--- ПОВНИЙ СПИСОК НОМЕРІВ ---");
            var roomsResult = _apiClient.GetRoomsAsync().GetAwaiter().GetResult();
            if (!roomsResult.Success || roomsResult.Data == null)
            {
                _logger.Print(roomsResult.Error ?? "Не вдалося отримати список номерів.");
                return;
            }

            if (roomsResult.Data.Count == 0)
            {
                _logger.Print("Список порожній.");
                return;
            }

            var roomView = roomsResult.Data
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
