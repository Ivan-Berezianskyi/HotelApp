using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IAccountLoader _accountLoader;
        private IReadOnlyList<IAccount>? _cachedAccounts;

        public AuthService(IAccountLoader accountLoader)
        {
            _accountLoader = accountLoader;
        }

        public IAccount? Authenticate(int roleId, string name, string password)
        {
            _cachedAccounts ??= _accountLoader.LoadAccountsFromDb();
            string normalizedName = name.Trim();

            IAccount? selectedAccount = _cachedAccounts?.FirstOrDefault(account =>
                IsRoleMatch(roleId, account)
                && string.Equals(account.Name, normalizedName, StringComparison.OrdinalIgnoreCase)
                && account.CheckPassword(password));

            if (selectedAccount == null)
            {
                return null;
            }

            return selectedAccount;
        }

        private static bool IsRoleMatch(int roleId, IAccount account)
        {
            return roleId switch
            {
                1 => account is IAdmin,
                2 => account is IClient,
                _ => false
            };
        }
    }
}
