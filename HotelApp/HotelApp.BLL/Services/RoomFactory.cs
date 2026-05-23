using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    public class RoomFactory : IRoomFactory
    {
        public bool TryCreate(string typeCode, int number, double price, out Room? room, out string? errorMessage)
        {
            if (!RoomCreatorRegistry.TryGetCreator(typeCode, out RoomCreator? creator) || creator == null)
            {
                room = null;
                errorMessage = "Unknown room type";
                return false;
            }

            room = creator.Create(number, price);
            errorMessage = null;
            return true;
        }
    }
}
