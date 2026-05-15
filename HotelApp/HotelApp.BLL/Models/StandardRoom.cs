namespace HotelApp.Models
{
    public class StandardRoom : Room
    {
        public StandardRoom(int num, double pr) : base(num, pr) { }
        public override double CalculateCost(int d) => (Price * d) + _servicesCost;
    }

}
