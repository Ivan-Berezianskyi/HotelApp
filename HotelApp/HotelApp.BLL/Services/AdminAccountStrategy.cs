using HotelApp.Interfaces;

namespace HotelApp.Services
{
    public class AdminAccountStrategy : IAccountStrategy
    {
        public bool IsApplicable(IAccount account)
        {
            return account is IAdmin;
        }

        public int GetRoleId()
        {
            return 1;
        }
    }
}
