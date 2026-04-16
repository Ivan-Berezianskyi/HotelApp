using HotelApp.UI;

namespace HotelApp.Interfaces
{
    internal interface IRoomTypeRegistry
    {
        IReadOnlyDictionary<string, RoomTypeDefinition> Definitions { get; }
        bool TryGet(string key, out RoomTypeDefinition? definition);
    }
}