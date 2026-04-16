using HotelApp.Interfaces;

namespace HotelApp.UI
{
    internal class AccountMenuFactory : IAccountMenuFactory
    {
        private readonly IAccountMenuRegistry _menuRegistry;

        public AccountMenuFactory(IAccountMenuRegistry menuRegistry)
        {
            _menuRegistry = menuRegistry;
        }

        public IMenu CreateMenu(IAccount account)
        {
            if (_menuRegistry.TryCreateMenu(account, out IMenu? menu) && menu != null)
            {
                return menu;
            }

            throw new ArgumentException("Невідомий тип акаунту");
        }
    }
}