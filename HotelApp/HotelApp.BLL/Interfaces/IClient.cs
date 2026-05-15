namespace HotelApp.Interfaces
{
    public interface IClient : IAccount
    {
        double Money { get; }
        void SyncMoney(double money);
    }
}