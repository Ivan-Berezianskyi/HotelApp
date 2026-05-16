using HotelApp.Data;
using HotelApp.Data.Entities;
using HotelApp.Interfaces;
using HotelApp.Models;
using HotelApp.Security;

namespace HotelApp.Services
{
    public class HotelAdminService : IHotelAdminService
    {
        private readonly Hotel _hotel;
        private readonly HotelDbContext _dbContext;

        public HotelAdminService(Hotel hotel, HotelDbContext dbContext)
        {
            _hotel = hotel;
            _dbContext = dbContext;
        }

        public bool TryGetRevenue(out double revenue, out string? errorMessage)
        {
            errorMessage = null;

            revenue = _hotel.Revenue;
            return true;
        }

        public bool TryAddRoom(Room room, out string? errorMessage)
        {
            return _hotel.AddRoom(room, out errorMessage);
        }

        public bool TryRemoveRoom(int number, out string? errorMessage)
        {
            return _hotel.RemoveRoom(number, out errorMessage);
        }

        public bool TryChangePassword(IAdmin admin, string currentPassword, string newPassword, out string? errorMessage)
        {
            if (!admin.CheckPassword(currentPassword))
            {
                errorMessage = "Помилка: невірний поточний пароль.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 4)
            {
                errorMessage = "Помилка: новий пароль має містити мінімум 4 символи.";
                return false;
            }

            string normalizedName = admin.Name.Trim().ToLowerInvariant();
            DbUser? user = _dbContext.Users.FirstOrDefault(item =>
                item.Name.ToLower() == normalizedName
                && item.Role.ToLower() == "admin");

            if (user == null)
            {
                errorMessage = "Помилка: адміністратора не знайдено в БД.";
                return false;
            }

            user.PasswordHash = PasswordHasher.Hash(newPassword);
            _dbContext.SaveChanges();

            admin.ChangePassword(currentPassword, newPassword);

            errorMessage = null;
            return true;
        }
    }
}