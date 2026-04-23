namespace HotelApp.Data.Entities
{
    internal class DbUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public double? Balance { get; set; }
    }
}