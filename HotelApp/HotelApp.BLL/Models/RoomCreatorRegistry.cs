namespace HotelApp.Models
{
    public static class RoomCreatorRegistry
    {
        public static IReadOnlyDictionary<string, RoomCreator> Creators { get; } =
            new Dictionary<string, RoomCreator>
            {
                ["1"] = new StandardRoomCreator(),
                ["2"] = new VipRoomCreator()
            };

        public static bool TryGetCreator(string code, out RoomCreator? creator)
        {
            if (Creators.TryGetValue(code, out RoomCreator? foundCreator))
            {
                creator = foundCreator;
                return true;
            }

            creator = null;
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
