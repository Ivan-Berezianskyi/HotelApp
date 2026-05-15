using HotelApp.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.UI
{
    internal class ApplicationRunner
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Run()
        {
            while (true)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var loginMenu = scope.ServiceProvider.GetRequiredService<ILoginMenu>();
                    var menuFactory = scope.ServiceProvider.GetRequiredService<IAccountMenuFactory>();

                    loginMenu.Display();

                    if (loginMenu.UserWantsToExit)
                    {
                        break;
                    }

                    UserSession? session = loginMenu.AuthenticatedUser;

                    if (session != null)
                    {
                        IMenu userMenu = menuFactory.CreateMenu(session);
                        userMenu.Display();
                        
                        loginMenu.ResetState();
                    }

                    Console.Clear();
                }
            }
        }
    }
}
