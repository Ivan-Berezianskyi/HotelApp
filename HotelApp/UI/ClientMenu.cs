using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.UI
{
    internal class ClientMenu : BaseHotelMenu
    {
        private readonly IClient _client;
        private readonly IHotelClientService _hotelClientService;

        public ClientMenu(Hotel hotel, IClient client, ILogger logger, IHotelClientService hotelClientService) : base(hotel, logger)
        {
            _client = client;
            _hotelClientService = hotelClientService;

        }
        public override void Display()
        {
            int action = 0;
            do
            {
                _logger.Print($"\n--- МЕНЮ КЛІЄНТА ({_client.Name}) ---");
                _logger.Print($"Баланс: {_client.Money} грн");
                _logger.Print("1. Список номерів | 2. Забронювати | 3. Оплатити | 0. Вихід");

                _logger.Print("Оберіть дію: ");
                TryReadNonNegativeNumber(out action);
                ProcessAction(action);
            } while (action != 0);

        }

        private void ProcessAction(int action)
        {
            switch (action)
            {
                case 1:
                    ShowRooms();
                    break;
                case 2:
                    BookRoom();
                    break;
                case 3:
                    PayForRoom();
                    break;
                case 0:
                    break;
                default:
                    _logger.Print("Невідома дія. Спробуйте ще раз.");
                    break;
            }
        }
        
        private void BookRoom()
        {
            _logger.Print("Введіть номер кімнати для бронювання: ");
            if (!TryReadPositiveNumber(out int roomNumber))
            {
                return;
            }
            
            if (!_hotelClientService.TryBookRoom(roomNumber, out string? errorMessage))
            {
                LogIfError(errorMessage);
                return;
            }

            _logger.Print($"Успіх: кімнату {roomNumber} заброньовано.");
        }

        private void PayForRoom()
        {
            IReadOnlyList<Room> rooms = _hotelClientService.GetMyOrders();
            if (rooms.Count == 0)
            {
                _logger.Print("У вас немає активних бронювань.");
                return;
            }

            string room_numbers = string.Join(", ", rooms.Select(room => room.Number));
            _logger.Print($"Ваші номери замовлення: {room_numbers}");
            _logger.Print("Введіть номер кімнати для оплати: ");
            if (!TryReadPositiveNumber(out int roomNumber)) { return; }

            _logger.Print("Днів прожито: ");
            if (!TryReadPositiveNumber(out int days)) { return; }

            if (!_hotelClientService.TryPayForRoom(roomNumber, days, out double paidAmount, out string? errorMessage))
            {
                LogIfError(errorMessage);
                return;
            }

            _logger.Print($"Успіх: сплачено {paidAmount} грн за кімнату {roomNumber}.");
        }
    }
}
