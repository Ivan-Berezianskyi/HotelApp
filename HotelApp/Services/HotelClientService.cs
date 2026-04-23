using HotelApp.Data;
using HotelApp.Data.Entities;
using HotelApp.Interfaces;
using HotelApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Services
{
    internal class HotelClientService : IHotelClientService
    {
        private readonly IClient _client;
        private readonly Hotel _hotel;
        private readonly HotelDbContext _dbContext;

        public HotelClientService(Hotel hotel, IClient client, HotelDbContext dbContext)
        {
            _hotel = hotel;
            _client = client;
            _dbContext = dbContext;
        }

        public bool TryBookRoom(int roomNumber, out string? errorMessage)
        {
            DbUser? user = GetClientUser();
            if (user == null)
            {
                errorMessage = "Помилка: клієнта не знайдено в БД.";
                return false;
            }

            Room? room = _hotel.FindRoomByNumber(roomNumber);
            if (room == null)
            {
                errorMessage = "Помилка: кімнату не знайдено.";
                return false;
            }

            if (_dbContext.Bookings.Any(booking =>
                    booking.UserId == user.Id
                    && booking.RoomNumber == roomNumber
                    && booking.IsActive))
            {
                errorMessage = "Помилка: ця кімната вже у ваших активних бронюваннях.";
                return false;
            }

            if (!_hotel.TrySetRoomOccupied(roomNumber, true, out errorMessage))
            {
                return false;
            }

            _dbContext.Bookings.Add(new DbBooking
            {
                UserId = user.Id,
                RoomNumber = roomNumber,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            });

            _dbContext.SaveChanges();
            
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

            DbUser? user = GetClientUser();
            if (user == null)
            {
                errorMessage = "Помилка: клієнта не знайдено в БД.";
                return false;
            }

            DbBooking? booking = _dbContext.Bookings
                .FirstOrDefault(item =>
                    item.UserId == user.Id
                    && item.RoomNumber == roomNumber
                    && item.IsActive);

            if (booking == null)
            {
                errorMessage = "Помилка: такої кімнати немає у ваших бронюваннях.";
                return false;
            }

            Room? room = _hotel.FindRoomByNumber(roomNumber);
            if (room == null)
            {
                errorMessage = "Помилка: кімнату не знайдено.";
                return false;
            }

            double cost = room.CalculateCost(stayDays);
            double currentBalance = user.Balance ?? 0;

            if (currentBalance < cost)
            {
                errorMessage = "Помилка: оплата не відбулась.";
                return false;
            }

            user.Balance = currentBalance - cost;
            booking.IsActive = false;
            booking.StayDays = stayDays;
            booking.PaidAmount = cost;
            booking.PaidUtc = DateTime.UtcNow;

            _dbContext.SaveChanges();

            _client.SyncMoney(user.Balance.Value);
            paidAmount = cost;

            if (!_hotel.TrySetRoomOccupied(roomNumber, false, out errorMessage))
            {
                return false;
            }

            _hotel.AddRevenue(paidAmount);
            errorMessage = null;

            return true;
        }

        public IReadOnlyList<Room> GetMyOrders()
        {
            DbUser? user = GetClientUser();
            if (user == null)
            {
                return new List<Room>().AsReadOnly();
            }

            List<int> roomNumbers = _dbContext.Bookings
                .AsNoTracking()
                .Where(item => item.UserId == user.Id && item.IsActive)
                .Select(item => item.RoomNumber)
                .ToList();

            List<Room> rooms = new List<Room>();
            foreach (int roomNumber in roomNumbers)
            {
                Room? room = _hotel.FindRoomByNumber(roomNumber);
                if (room != null)
                {
                    rooms.Add(room);
                }
            }

            return rooms.AsReadOnly();
        }

        private DbUser? GetClientUser()
        {
            return _dbContext.Users.FirstOrDefault(user =>
                user.Name == _client.Name
                && user.Role == "client");
        }
    }
}