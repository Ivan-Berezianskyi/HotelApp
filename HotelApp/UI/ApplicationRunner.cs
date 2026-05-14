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

                    IAccount? currentAccount = loginMenu.AuthenticatedAccount;

                    if (currentAccount != null)
                    {
                        IMenu userMenu = menuFactory.CreateMenu(currentAccount);
                        userMenu.Display();
                        
                        loginMenu.ResetState();
                    }

                    Console.Clear();
                }
            }
        }
    }
}
