using HotelApp.Models;

namespace HotelApp.Interfaces
{
    internal interface IClient : IAccount
    {
        double Money { get; }
        bool Order(Room room);
        IReadOnlyList<Room> GetMyOrders();
        bool Pay(Room room, int stayDays, out double paidAmount);
    }
}