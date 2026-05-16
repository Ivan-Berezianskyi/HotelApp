using HotelApp.Models;

namespace HotelApp.Interfaces
{
    public interface IHotelAdminService
    {
        bool TryAddRoom(Room room, out string? errorMessage);
        bool TryRemoveRoom(int number, out string? errorMessage);
        bool TryGetRevenue(out double revenue, out string? errorMessage);
        bool TryChangePassword(IAdmin admin, string currentPassword, string newPassword, out string? errorMessage);
        bool TryChangePasswordByName(string name, string currentPassword, string newPassword, out string? errorMessage);
    }
}
