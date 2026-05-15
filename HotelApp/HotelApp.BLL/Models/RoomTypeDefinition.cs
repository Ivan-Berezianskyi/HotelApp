namespace HotelApp.Models
{
    public sealed class RoomTypeDefinition
    {
        public string Name { get; }
        public Func<int, double, Room> Factory { get; }

        public RoomTypeDefinition(string name, Func<int, double, Room> factory)
        {
            Name = name;
            Factory = factory;
        }
    }
}
