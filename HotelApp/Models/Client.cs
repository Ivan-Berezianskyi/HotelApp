using HotelApp.Interfaces;

namespace HotelApp.Models
{
    internal class Client : Account, IClient
    {
        public double Money { get; private set; }

        public Client(string name, string password, double money) : base(name, password)
        {
            Money = money;
        }

        public void SyncMoney(double money)
        {
            Money = money;
        }
    }
}
