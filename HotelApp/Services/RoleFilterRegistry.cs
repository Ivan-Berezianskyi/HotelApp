using HotelApp.Interfaces;

namespace HotelApp.Services
{
    internal class RoleFilterRegistry : IRoleFilterRegistry
    {
        private readonly IReadOnlyDictionary<int, Func<IAccount, bool>> _roleFilters;

        public RoleFilterRegistry()
        {
            _roleFilters = new Dictionary<int, Func<IAccount, bool>>
            {
                [1] = account => account is IAdmin,
                [2] = account => account is IClient
            };
        }

        public bool TryGetRoleFilter(int roleId, out Func<IAccount, bool>? roleFilter)
        {
            if (_roleFilters.TryGetValue(roleId, out Func<IAccount, bool>? foundFilter))
            {
                roleFilter = foundFilter;
                return true;
            }

            roleFilter = null;
            return false;
        }
    }
}