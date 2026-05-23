namespace HotelApp.Models
{
    public sealed class VipRoomCreator : RoomCreator
    {
        public override string Code => "2";
        public override string Name => "VIP";

        public override Room Create(int number, double price)
        {
            return new VIPRoom(number, price);
        }
    }
}
