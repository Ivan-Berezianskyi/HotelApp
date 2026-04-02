using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    internal class HotelClientService : IHotelClientService
    {
        private readonly IClient _client;
        private readonly Hotel _hotel;

        public HotelClientService(Hotel hotel, IClient client)
        {
            _hotel = hotel;
            _client = client;
        }

        public bool TryBookRoom(int roomNumber, out string? errorMessage)
        {
            Room? room = _hotel.FindRoomByNumber(roomNumber);
            if (room == null)
            {
                errorMessage = "Помилка: кімнату не знайдено.";
                return false;
            }

            if (!_client.Order(room))
            {
                errorMessage = "Помилка: не вдалося забронювати кімнату.";
                return false;
            }
            
            errorMessage = null;
            
            return true;
        }

        public bool TryPayForRoom(int roomNumber, int stayDays, out double paidAmount, out string? errorMessage)
        {
            paidAmount = 0;

            if (stayDays <= 0)
            {
                errorMessage = "Помилка: кількість днів має бути додатною.";
                return false;
            }

            Room? room = _client.GetMyOrders().FirstOrDefault(r => r.Number == roomNumber);
            if (room == null)
            {
                errorMessage = "Помилка: такої кімнати немає у ваших бронюваннях.";
                return false;
            }

            if (!_client.Pay(room, stayDays, out paidAmount))
            {
                errorMessage = "Помилка: оплата не відбулась.";
                return false;
            }

            _hotel.AddRevenue(paidAmount);
            errorMessage = null;

            return true;
        }

        public IReadOnlyList<Room> GetMyOrders()
        {
            return _client.GetMyOrders();
        }
    }
}