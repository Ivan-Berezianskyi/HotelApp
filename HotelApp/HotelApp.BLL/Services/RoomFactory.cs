using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    public class RoomFactory : IRoomFactory
    {
        public bool TryCreate(string typeCode, int number, double price, out Room? room, out string? errorMessage)
        {
            if (!RoomTypeRegistry.TryGet(typeCode, out RoomTypeDefinition? definition) || definition == null)
            {
                room = null;
                errorMessage = "Unknown room type";
                return false;
            }

            room = definition.Factory(number, price);
            errorMessage = null;
            return true;
        }
    }
}
