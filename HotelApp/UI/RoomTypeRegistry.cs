using HotelApp.Interfaces;

namespace HotelApp.UI
{
    internal class RoomTypeRegistry : IRoomTypeRegistry
    {
        public IReadOnlyDictionary<string, RoomTypeDefinition> Definitions { get; }

        public RoomTypeRegistry(IReadOnlyDictionary<string, RoomTypeDefinition> definitions)
        {
            Definitions = new Dictionary<string, RoomTypeDefinition>(definitions);
        }

        public bool TryGet(string key, out RoomTypeDefinition? definition)
        {
            if (Definitions.TryGetValue(key, out RoomTypeDefinition? foundDefinition))
            {
                definition = foundDefinition;
                return true;
            }

            definition = null;
            return false;
        }
    }
}