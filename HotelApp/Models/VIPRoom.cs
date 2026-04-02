namespace HotelApp.Models
{
    internal class VIPRoom : Room
    {
        public VIPRoom(int num, double pr) : base(num, pr) { }
        public override double CalculateCost(int d) => (Price * d * 1.5) + _servicesCost;
    }
}
