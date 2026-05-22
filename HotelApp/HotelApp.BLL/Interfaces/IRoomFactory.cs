using HotelApp.Models;

namespace HotelApp.Interfaces
{
    public interface IRoomFactory
    {
        bool TryCreate(string typeCode, int number, double price, out Room? room, out string? errorMessage);
    }
}
