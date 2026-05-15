namespace HotelApp.Models
{
    public static class RoomTypeRegistry
    {
        public static IReadOnlyDictionary<string, RoomTypeDefinition> Definitions { get; } =
            new Dictionary<string, RoomTypeDefinition>
            {
                ["1"] = new RoomTypeDefinition("Standard", (number, price) => new StandardRoom(number, price)),
                ["2"] = new RoomTypeDefinition("VIP", (number, price) => new VIPRoom(number, price))
            };

        public static bool TryGet(string key, out RoomTypeDefinition? definition)
        {
            if (Definitions.TryGetValue(key, out RoomTypeDefinition? foundDefinition))
            {
                definition = foundDefinition;
                return true;
            }

            definition = null;
            return false;
        }

        public static bool TryGetCode(Room room, out string? code)
        {
            if (room is StandardRoom)
            {
                code = "1";
                return true;
            }

            if (room is VIPRoom)
            {
                code = "2";
                return true;
            }

            code = null;
            return false;
        }
    }
}
