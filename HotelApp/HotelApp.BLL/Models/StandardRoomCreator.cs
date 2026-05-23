namespace HotelApp.Models
{
    public sealed class StandardRoomCreator : RoomCreator
    {
        public override string Code => "1";
        public override string Name => "Standard";

        public override Room Create(int number, double price)
        {
            return new StandardRoom(number, price);
        }
    }
}
