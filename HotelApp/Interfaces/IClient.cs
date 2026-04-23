namespace HotelApp.Interfaces
{
    internal interface IClient : IAccount
    {
        double Money { get; }
        void SyncMoney(double money);
    }
}