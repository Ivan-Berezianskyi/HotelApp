using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.UI
{
    internal class AccountMenuFactory : IAccountMenuFactory
    {
        private readonly Hotel _hotel;
        private readonly ILogger _logger;
        private readonly IHotelAdminService _hotelAdminService;
        private readonly Func<IClient, IHotelClientService> _clientServiceFactory;

        public AccountMenuFactory(
            Hotel hotel,
            ILogger logger,
            IHotelAdminService hotelAdminService,
            Func<IClient, IHotelClientService> clientServiceFactory)
        {
            _hotel = hotel;
            _logger = logger;
            _hotelAdminService = hotelAdminService;
            _clientServiceFactory = clientServiceFactory;
        }

        public IMenu CreateMenu(IAccount account)
        {
            if (account is IAdmin admin)
            {
                return new AdminMenu(_hotel, admin, _logger, _hotelAdminService);
            }
            else if (account is IClient client)
            {
                IHotelClientService hotelClientService = _clientServiceFactory(client);
                return new ClientMenu(_hotel, client, _logger, hotelClientService);
            }

            throw new ArgumentException("Невідомий тип акаунту");
        }
    }
}