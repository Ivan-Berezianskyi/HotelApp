namespace HotelApp.Interfaces
{
    public interface IAccountLoader
    {
        List<IAccount> LoadAccountsFromDb();
    }
}
