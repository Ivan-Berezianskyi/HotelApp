namespace HotelApp.Interfaces
{
    internal interface IAccountLoader
    {
        List<IAccount> LoadAccountsFromDb();
    }
}
