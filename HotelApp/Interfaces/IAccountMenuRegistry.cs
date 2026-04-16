namespace HotelApp.Interfaces
{
    internal interface IAccountMenuRegistry
    {
        bool TryCreateMenu(IAccount account, out IMenu? menu);
    }
}