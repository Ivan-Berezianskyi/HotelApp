using HotelApp.Models;

namespace HotelApp.Interfaces
{
    internal interface IHotelClientService
    {
        bool TryBookRoom(int roomNumber, out string? errorMessage);
        bool TryPayForRoom(int roomNumber, int stayDays, out double paidAmount, out string? errorMessage);
        IReadOnlyList<Room> GetMyOrders();
    }
}