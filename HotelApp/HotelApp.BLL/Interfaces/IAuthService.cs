namespace HotelApp.Interfaces
{
    public interface IAuthService
    {
        IAccount? Authenticate(int roleId, string name, string password);
    }
}