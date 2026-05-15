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

        public T GetTypedAccount<T>(IAccount account) where T : class, IAccount
        {
            if (account is IClient client)
            {
                return client as T ?? throw new InvalidOperationException("Cannot cast client account to requested type");
            }

            throw new InvalidOperationException($"Account is not client type");
        }
    }
}
