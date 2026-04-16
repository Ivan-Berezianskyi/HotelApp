using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IReadOnlyList<IAccount> _accounts;
        private readonly IRoleFilterRegistry _roleFilterRegistry;

        public AuthService(IEnumerable<IAccount> accounts, IRoleFilterRegistry roleFilterRegistry)
        {
            _accounts = accounts.ToList();
            _roleFilterRegistry = roleFilterRegistry;
        }

        public IAccount? Authenticate(int roleId, string name, string password)
        {
            if (!_roleFilterRegistry.TryGetRoleFilter(roleId, out Func<IAccount, bool>? roleFilter) || roleFilter == null)
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
