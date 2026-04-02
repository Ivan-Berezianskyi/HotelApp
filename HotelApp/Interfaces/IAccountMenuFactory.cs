namespace HotelApp.Interfaces
{
    internal interface IAccountMenuFactory
    {
        IMenu CreateMenu(IAccount account);
    }
}