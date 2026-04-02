namespace HotelApp.Models
{
    internal class StandardRoom : Room
    {
        public StandardRoom(int num, double pr) : base(num, pr) { }
        public override double CalculateCost(int d) => (Price * d) + _servicesCost;
    }

}
