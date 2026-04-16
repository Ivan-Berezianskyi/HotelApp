using HotelApp.Models;
using HotelApp.Services;
using HotelApp.UI;
using HotelApp.Interfaces;
using HotelApp.Infrastructure;

namespace HotelBookingSystem
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Hotel myHotel = new Hotel();
            IAdmin admin = new Admin("Головний Адмін", "admin");
            IClient client = new Client("Олександр", "1234", 10000);
            List<IAccount> accounts = new List<IAccount> { admin, client };
            IRoleFilterRegistry roleFilterRegistry = new RoleFilterRegistry();
            IAuthService authService = new AuthService(accounts, roleFilterRegistry);
            ILogger logger = new ConsoleLogger();
            IHotelAdminService hotelAdminService = new HotelAdminService(myHotel);
            Func<IClient, IHotelClientService> clientServiceFactory =
                currentClient => new HotelClientService(myHotel, currentClient);
            IRoomTypeRegistry roomTypeRegistry = new RoomTypeRegistry(
                new Dictionary<string, RoomTypeDefinition>
                {
                    ["1"] = new RoomTypeDefinition("Standard", (number, price) => new StandardRoom(number, price)),
                    ["2"] = new RoomTypeDefinition("VIP", (number, price) => new VIPRoom(number, price))
                });

            IAccountMenuRegistry accountMenuRegistry = new AccountMenuRegistry(
                myHotel,
                logger,
                hotelAdminService,
                clientServiceFactory,
                roomTypeRegistry);

            IAccountMenuFactory menuFactory = new AccountMenuFactory(accountMenuRegistry);

            while (true)
            {
                ILoginMenu loginMenu = new LoginMenu(authService, logger);
                loginMenu.Display();

                IAccount? currentAccount = loginMenu.AuthenticatedAccount;

                if (currentAccount != null)
                {
                    IMenu userMenu = menuFactory.CreateMenu(currentAccount);
                    
                    userMenu.Display();
                }

                Console.Clear();
            }
        }
    }
}