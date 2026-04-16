namespace HotelApp.Interfaces
{
    internal interface IRoleFilterRegistry
    {
        bool TryGetRoleFilter(int roleId, out Func<IAccount, bool>? roleFilter);
    }
}