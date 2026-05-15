namespace HotelApp.Interfaces
{
    public interface IAccount
    {
        string Name { get; }
        bool CheckPassword(string pass);
    }
}