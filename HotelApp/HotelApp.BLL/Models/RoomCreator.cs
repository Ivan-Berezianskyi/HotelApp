namespace HotelApp.Models
{
    public abstract class RoomCreator
    {
        public abstract string Code { get; }
        public abstract string Name { get; }
        public abstract Room Create(int number, double price);
    }
}
