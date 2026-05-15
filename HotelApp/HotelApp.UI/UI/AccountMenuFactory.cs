using HotelApp.Interfaces;
using HotelApp.UI.Interfaces;

namespace HotelApp.UI
{
    internal class AccountMenuFactory : IAccountMenuFactory
    {
        private readonly ILogger _logger;
        private readonly IHotelApiClient _apiClient;

        public AccountMenuFactory(
            ILogger logger,
            IHotelApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public IMenu CreateMenu(UserSession session)
        {
            return session.RoleId switch
            {
                1 => CreateAdminMenu(session),
                2 => CreateClientMenu(session),
                _ => throw new ArgumentException("Невідомий тип акаунту")
            };
        }

        private IMenu CreateAdminMenu(UserSession session)
        {
            return new AdminMenu(session, _logger, _apiClient);
        }

        private IMenu CreateClientMenu(UserSession session)
        {
            return new ClientMenu(session, _logger, _apiClient);
        }
    }
}
