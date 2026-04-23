namespace HotelApp.Data.Entities
{
    internal class DbBooking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomNumber { get; set; }
        public bool IsActive { get; set; }
        public int? StayDays { get; set; }
        public double? PaidAmount { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? PaidUtc { get; set; }
    }
}