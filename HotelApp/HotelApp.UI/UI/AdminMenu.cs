using HotelApp.Interfaces;
using HotelApp.UI.Api;
using HotelApp.UI.Interfaces;

namespace HotelApp.UI
{
    internal class AdminMenu : BaseHotelMenu
    {
        public AdminMenu(
            UserSession session,
            ILogger logger,
            IHotelApiClient apiClient) : base(session, logger, apiClient)
        {
        }
        
        public override void Display()
        {
            int action;
            do
            {
                _logger.Print($"\n--- ПАНЕЛЬ АДМІНІСТРАТОРА ({_session.Name}) ---");
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
            
            TryReadRoomType(out (string Key, string Name)? roomTypeDefinition);
            if (roomTypeDefinition == null) { return; }

            var roomType = roomTypeDefinition.Value;

            _logger.Print("Ціна: ");
            if (!TryReadPositiveNumber(out double price)) { return; }

            var request = new AddRoomRequest(roomNumber, price, roomType.Key);
            var result = _apiClient.AddRoomAsync(request).GetAwaiter().GetResult();
            if (!result.Success)
            {
                LogIfError(result.Error);
                return;
            }

            _logger.Print($"Успіх: номер {roomNumber} успішно додано.");
        }

        private void RemoveRoom()
        {
            _logger.Print("Введіть номер для видалення: ");
            
            if(!TryReadPositiveNumber(out int roomNumber)) { return; }

            var result = _apiClient.RemoveRoomAsync(roomNumber).GetAwaiter().GetResult();
            if (!result.Success)
            {
                LogIfError(result.Error);
                return;
            }

            _logger.Print($"Успіх: номер {roomNumber} успішно видалено.");

        }

        private void ShowRevenue()
        {
            var result = _apiClient.GetRevenueAsync().GetAwaiter().GetResult();
            if (!result.Success || result.Data == null)
            {
                LogIfError(result.Error);
                return;
            }

            _logger.Print($"Загальний прибуток готелю: {result.Data.Revenue}");
        }

        private void PrintAvailableRoomTypes()
        {
            string options = string.Join(
                ", ",
                RoomTypeDefinitions.Select(pair => $"{pair.Key}-{pair.Value}"));
            _logger.Print($"Тип ({options}): ");
        }

        private void TryReadRoomType(out (string Key, string Name)? roomTypeDefinition)
        {
            string? type = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(type) || !RoomTypeDefinitions.TryGetValue(type, out string? name))
            {
                roomTypeDefinition = null;
                _logger.Print("Помилка: тип кімнати має бути одним зі значень");
                
                return;
            }

            roomTypeDefinition = (type, name);
        }

        private void ChangePassword()
        {
            _logger.Print("Введіть поточний пароль: ");
            string currentPassword = Console.ReadLine() ?? string.Empty;

            _logger.Print("Введіть новий пароль: ");
            string newPassword = Console.ReadLine() ?? string.Empty;

            var request = new ChangePasswordRequest(currentPassword, newPassword);
            var result = _apiClient.ChangePasswordAsync(_session.Name, request).GetAwaiter().GetResult();
            if (!result.Success)
            {
                LogIfError(result.Error);
                return;
            }

            _logger.Print("Успіх: пароль змінено.");
        }

        private static readonly IReadOnlyDictionary<string, string> RoomTypeDefinitions =
            new Dictionary<string, string>
            {
                ["1"] = "Standard",
                ["2"] = "VIP"
            };
    }
}
