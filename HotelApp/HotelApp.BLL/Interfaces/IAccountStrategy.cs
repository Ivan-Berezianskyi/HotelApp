namespace HotelApp.Interfaces
{
    public interface IAccountStrategy
    {
        bool IsApplicable(IAccount account);
        int GetRoleId();
        T GetTypedAccount<T>(IAccount account) where T : class, IAccount;
    }
}
