using HotelApp.Interfaces;
using HotelApp.UI.Api;
using HotelApp.UI.Interfaces;

namespace HotelApp.UI
{
    internal class ClientMenu : BaseHotelMenu
    {
        public ClientMenu(UserSession session, ILogger logger, IHotelApiClient apiClient) : base(session, logger, apiClient)
        {
        }
        public override void Display()
        {
            int action;
            do
            {
                string balance = _session.Balance.HasValue ? _session.Balance.Value.ToString() : "N/A";
                _logger.Print($"\n--- МЕНЮ КЛІЄНТА ({_session.Name}) ---");
                _logger.Print($"Баланс: {balance} грн");
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

            var result = _apiClient.BookRoomAsync(_session.Name, roomNumber).GetAwaiter().GetResult();
            if (!result.Success)
            {
                LogIfError(result.Error);
                return;
            }

            _logger.Print($"Успіх: кімнату {roomNumber} заброньовано.");
        }

        private void PayForRoom()
        {
            var ordersResult = _apiClient.GetClientOrdersAsync(_session.Name).GetAwaiter().GetResult();
            if (!ordersResult.Success || ordersResult.Data == null)
            {
                LogIfError(ordersResult.Error);
                return;
            }

            IReadOnlyList<RoomDto> rooms = ordersResult.Data;
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

            var payResult = _apiClient.PayForRoomAsync(_session.Name, roomNumber, days).GetAwaiter().GetResult();
            if (!payResult.Success || payResult.Data == null)
            {
                LogIfError(payResult.Error);
                return;
            }

            _session.SetBalance(payResult.Data.Balance);
            _logger.Print($"Успіх: сплачено {payResult.Data.PaidAmount} грн за кімнату {roomNumber}.");
        }
    }
}
