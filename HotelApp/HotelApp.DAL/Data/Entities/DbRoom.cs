namespace HotelApp.Data.Entities
{
    public class DbRoom
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string RoomTypeCode { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsOccupied { get; set; }
        public ICollection<DbBooking> Bookings { get; set; } = new List<DbBooking>();
    }
}