using HotelApp.Interfaces;

namespace HotelApp.Services
{
    public class ClientAccountStrategy : IAccountStrategy
    {
        public bool IsApplicable(IAccount account)
        {
            return account is IClient;
        }

        public int GetRoleId()
        {
            return 2;
        }
    }
}
