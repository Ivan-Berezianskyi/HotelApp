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

        public T GetTypedAccount<T>(IAccount account) where T : class, IAccount
        {
            if (account is IAdmin admin)
            {
                return admin as T ?? throw new InvalidOperationException("Cannot cast admin account to requested type");
            }

            throw new InvalidOperationException($"Account is not admin type");
        }
    }
}
