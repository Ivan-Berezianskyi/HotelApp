namespace HotelApp.UI
{
    internal class UserSession
    {
        public int RoleId { get; }
        public string Name { get; }
        public double? Balance { get; private set; }

        public UserSession(int roleId, string name, double? balance)
        {
            RoleId = roleId;
            Name = name;
            Balance = balance;
        }

        public void SetBalance(double? balance)
        {
            Balance = balance;
        }
    }
}
