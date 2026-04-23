using HotelApp.Interfaces;
using HotelApp.Models;
using HotelApp.UI;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Data
{
    internal static class SqliteRoomTypeRegistryFactory
    {
        public static IRoomTypeRegistry Create(HotelDbContext dbContext)
        {
            Dictionary<string, RoomTypeDefinition> definitions = new Dictionary<string, RoomTypeDefinition>();
            List<Entities.DbRoomType> roomTypes = dbContext.RoomTypes.AsNoTracking().ToList();

            foreach (Entities.DbRoomType roomType in roomTypes)
            {
                if (roomType.Code == "1")
                {
                    definitions[roomType.Code] =
                        new RoomTypeDefinition(roomType.Name, (number, price) => new StandardRoom(number, price));
                }
                else if (roomType.Code == "2")
                {
                    definitions[roomType.Code] =
                        new RoomTypeDefinition(roomType.Name, (number, price) => new VIPRoom(number, price));
                }
            }

            if (definitions.Count == 0)
            {
                definitions["1"] = new RoomTypeDefinition("Standard", (number, price) => new StandardRoom(number, price));
                definitions["2"] = new RoomTypeDefinition("VIP", (number, price) => new VIPRoom(number, price));
            }

            return new RoomTypeRegistry(definitions);
        }
    }
}