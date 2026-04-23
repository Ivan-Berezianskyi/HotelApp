using HotelApp.Models;
using HotelApp.Services;
using HotelApp.UI;
using HotelApp.Interfaces;
using HotelApp.Infrastructure;
using HotelApp.Data;
using HotelApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            using HotelDbContext dbContext = new HotelDbContext("Data Source=hotelapp.db");
            SqliteBootstrapper.EnsureCreatedAndSeed(dbContext);

            Hotel myHotel = new Hotel(dbContext);
            List<IAccount> accounts = LoadAccountsFromDb(dbContext);
            IRoleFilterRegistry roleFilterRegistry = new RoleFilterRegistry();
            IAuthService authService = new AuthService(accounts, roleFilterRegistry);
            ILogger logger = new ConsoleLogger();
            IHotelAdminService hotelAdminService = new HotelAdminService(myHotel);
            Func<IClient, IHotelClientService> clientServiceFactory =
                currentClient => new HotelClientService(myHotel, currentClient, dbContext);
            IRoomTypeRegistry roomTypeRegistry = SqliteRoomTypeRegistryFactory.Create(dbContext);

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

        private static List<IAccount> LoadAccountsFromDb(HotelDbContext dbContext)
        {
            List<IAccount> accounts = new List<IAccount>();

            List<DbUser> users = dbContext.Users.AsNoTracking().ToList();
            foreach (DbUser user in users)
            {
                if (string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    accounts.Add(new Admin(user.Name, user.PasswordHash));
                    continue;
                }

                if (string.Equals(user.Role, "client", StringComparison.OrdinalIgnoreCase))
                {
                    accounts.Add(new Client(user.Name, user.PasswordHash, user.Balance ?? 0));
                }
            }

            return accounts;
        }
    }
}