using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.UI
{
    internal class AccountMenuRegistry : IAccountMenuRegistry
    {
        private readonly IReadOnlyList<Func<IAccount, IMenu?>> _menuBuilders;

        public AccountMenuRegistry(
            Hotel hotel,
            ILogger logger,
            IHotelAdminService hotelAdminService,
            Func<IClient, IHotelClientService> clientServiceFactory,
            IRoomTypeRegistry roomTypeRegistry)
        {
            _menuBuilders = new List<Func<IAccount, IMenu?>>
            {
                account =>
                {
                    if (account is not IAdmin admin)
                    {
                        return null;
                    }

                    return new AdminMenu(hotel, admin, logger, hotelAdminService, roomTypeRegistry);
                },
                account =>
                {
                    if (account is not IClient client)
                    {
                        return null;
                    }

                    IHotelClientService clientService = clientServiceFactory(client);
                    return new ClientMenu(hotel, client, logger, clientService);
                }
            };
        }

        public bool TryCreateMenu(IAccount account, out IMenu? menu)
        {
            foreach (Func<IAccount, IMenu?> buildMenu in _menuBuilders)
            {
                IMenu? candidate = buildMenu(account);
                if (candidate != null)
                {
                    menu = candidate;
                    return true;
                }
            }

            menu = null;
            return false;
        }
    }
}