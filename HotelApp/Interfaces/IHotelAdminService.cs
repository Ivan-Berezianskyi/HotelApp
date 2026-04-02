using HotelApp.Models;

namespace HotelApp.Interfaces
{
    internal interface IHotelAdminService
    {
        bool TryAddRoom(Room room, out string? errorMessage);
        bool TryRemoveRoom(int number, out string? errorMessage);
        bool TryGetRevenue(out double revenue, out string? errorMessage);
    }
}
