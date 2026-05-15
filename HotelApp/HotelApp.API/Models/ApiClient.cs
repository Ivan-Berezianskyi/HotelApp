using HotelApp.Interfaces;

namespace HotelApp.API.Models
{
    public class ApiClient : IClient
    {
        public string Name { get; }
        public double Money { get; private set; }

        public ApiClient(string name, double money)
        {
            Name = name;
            Money = money;
        }

        public bool CheckPassword(string pass)
        {
            return false;
        }

        public void SyncMoney(double money)
        {
            Money = money;
        }
    }
}
