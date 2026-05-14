using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.UI
{
    internal class AdminMenu : BaseHotelMenu
    {
        private readonly IAdmin _admin;
        private readonly IHotelAdminService _hotelAdminService;

        public AdminMenu(
            Hotel hotel,
            IAdmin admin,
            ILogger logger,
            IHotelAdminService hotelAdminService) : base(hotel, logger)
        {
            _admin = admin;
            _hotelAdminService = hotelAdminService;
        }
        
        public override void Display()
        {
            int action;
            do
            {
                _logger.Print($"\n--- ПАНЕЛЬ АДМІНІСТРАТОРА ({_admin.Name}) ---");
                _logger.Print("1. Список номерів | 2. Додати номер | 3. Видалити номер | 4. Прибуток | 5. Змінити пароль | 0. Вихід");
                _logger.Print("Оберіть дію: ");

                TryReadNonNegativeNumber(out action);
                HandleAction(action);
            } while (action != 0);

        }
       
        private void HandleAction(int action)
        {
            switch (action)
            {
                case 1:
                    ShowRooms();
                    break;
                case 2:
                    AddRoom();
                    break;
                case 3:
                    RemoveRoom();
                    break;
                case 4:
                    ShowRevenue();
                    break;
                case 5:
                    ChangePassword();
                    break;
                case 0:
                    break;
                default:
                    _logger.Print("Невідома дія. Спробуйте ще раз.");
                    break;
            }
        }

        private void AddRoom()
        {
            _logger.Print("Номер: ");
            if(!TryReadPositiveNumber(out int roomNumber)) { return; }

            PrintAvailableRoomTypes();
            
            TryReadRoomType(out RoomTypeDefinition? roomTypeDefinition);
            if (roomTypeDefinition == null) { return; }

            _logger.Print("Ціна: ");
            if (!TryReadPositiveNumber(out double price)) { return; }

            Room room = roomTypeDefinition.Factory(roomNumber, price);
            _hotelAdminService.TryAddRoom(room, out string? errorMessage);
            if (LogIfError(errorMessage)) return;

            _logger.Print($"Успіх: номер {roomNumber} успішно додано.");
        }

        private void RemoveRoom()
        {
            _logger.Print("Введіть номер для видалення: ");
            
            if(!TryReadPositiveNumber(out int roomNumber)) { return; }

            _hotelAdminService.TryRemoveRoom(roomNumber, out string? errorMessage);

            if (LogIfError(errorMessage)) return;

            _logger.Print($"Успіх: номер {roomNumber} успішно видалено.");

        }

        private void ShowRevenue()
        {
            _hotelAdminService.TryGetRevenue(out double revenue, out string? errorMessage);

            if (LogIfError(errorMessage)) return;

            _logger.Print($"Загальний прибуток готелю: {revenue}");
        }

        private void PrintAvailableRoomTypes()
        {
            string options = string.Join(
                ", ",
                RoomTypeRegistry.Definitions.Select(pair => $"{pair.Key}-{pair.Value.Name}"));
            _logger.Print($"Тип ({options}): ");
        }

        private void TryReadRoomType(out RoomTypeDefinition? roomTypeDefinition)
        {
            string? type = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(type) || !RoomTypeRegistry.TryGet(type, out roomTypeDefinition))
            {
                roomTypeDefinition = null;
                _logger.Print("Помилка: тип кімнати має бути одним зі значень");
                
                return;
            }
        }

        private void ChangePassword()
        {
            _logger.Print("Введіть поточний пароль: ");
            string currentPassword = Console.ReadLine() ?? string.Empty;

            _logger.Print("Введіть новий пароль: ");
            string newPassword = Console.ReadLine() ?? string.Empty;

            if (!_hotelAdminService.TryChangePassword(_admin, currentPassword, newPassword, out string? errorMessage))
            {
                LogIfError(errorMessage);
                return;
            }

            _logger.Print("Успіх: пароль змінено.");
        }
    }
}
