using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IReadOnlyList<IAccount> _accounts;

        public AuthService(IEnumerable<IAccount> accounts)
        {
            _accounts = accounts.ToList();
        }

        public IAccount? Authenticate(int roleId, string name, string password)
        {
            Func<IAccount, bool>? roleFilter = roleId switch
            {
                1 => account => account is IAdmin,
                2 => account => account is IClient,
                _ => null
            };

            if (roleFilter == null)
            {
                return null;
            }

            string normalizedName = name.Trim();

            IAccount? selectedAccount = _accounts.FirstOrDefault(account =>
                roleFilter(account)
                && string.Equals(account.Name, normalizedName, StringComparison.OrdinalIgnoreCase)
                && account.CheckPassword(password));

            if (selectedAccount == null)
            {
                return null;
            }

            return selectedAccount;
        }
    }
}
