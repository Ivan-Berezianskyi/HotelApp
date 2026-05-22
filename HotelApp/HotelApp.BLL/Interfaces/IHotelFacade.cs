using HotelApp.Models;

namespace HotelApp.Interfaces
{
    public interface IHotelFacade
    {
        IAccount? Authenticate(int roleId, string name, string password);

        bool TryGetRevenue(out double revenue, out string? errorMessage);
        bool TryAddRoom(string typeCode, int number, double price, out string? errorMessage);
        bool TryRemoveRoom(int number, out string? errorMessage);
        bool TryChangePasswordByName(string name, string currentPassword, string newPassword, out string? errorMessage);

        double GetClientBalance(IClient client);
        IReadOnlyList<Room> GetClientOrders(IClient client);
        bool TryBookRoom(IClient client, int roomNumber, out string? errorMessage);
        bool TryPayForRoom(IClient client, int roomNumber, int stayDays, out double paidAmount, out string? errorMessage);
    }
}
