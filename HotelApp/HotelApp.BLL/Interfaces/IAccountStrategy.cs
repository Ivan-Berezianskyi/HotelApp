namespace HotelApp.Interfaces
{
    public interface IAccountStrategy
    {
        bool IsApplicable(IAccount account);
        int GetRoleId();
    }
}
