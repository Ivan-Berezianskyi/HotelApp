namespace HotelApp.Interfaces
{
    internal interface IAuthService
    {
        IAccount? Authenticate(int roleId, string name, string password);
    }
}