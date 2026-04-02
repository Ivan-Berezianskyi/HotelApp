namespace HotelApp.Models
{
    internal abstract class Room
    {
        public int Number { get; }
        public bool IsOccupied { get; protected set; }
        public double Price { get; }
        protected double _servicesCost = 0;

        public Room(int num, double pr) { Number = num; Price = pr; }
        public abstract double CalculateCost(int days);
        public void Book() => IsOccupied = true;
        public void DeBook() { IsOccupied = false; _servicesCost = 0; }
    }
}
