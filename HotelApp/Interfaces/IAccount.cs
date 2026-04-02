namespace HotelApp.Interfaces
{
    internal interface IAccount
    {
        string Name { get; }
        bool CheckPassword(string pass);
    }
}