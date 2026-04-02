namespace HotelApp.Interfaces
{
    internal interface ILoginMenu : IMenu
    {
        IAccount? AuthenticatedAccount { get; }
    }
}