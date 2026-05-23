using HotelApp.Data;
using HotelApp.Data.Entities;
using HotelApp.Models;
using HotelApp.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.Bootstrap
{
    public static class DatabaseBootstrapper
    {
        public static void EnsureCreatedAndSeed(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            EnsureCreatedAndSeed(dbContext, config);
        }

        public static void EnsureCreatedAndSeed(HotelDbContext dbContext, IConfiguration config)
        {
            dbContext.Database.EnsureCreated();

            if (!dbContext.RoomTypes.Any())
            {
                foreach (KeyValuePair<string, RoomCreator> entry in RoomCreatorRegistry.Creators)
                {
                    dbContext.RoomTypes.Add(new DbRoomType { Code = entry.Key, Name = entry.Value.Name });
                }
            }

            if (!dbContext.Users.Any())
            {
                string adminName = config["SeedData:AdminName"] ?? "Head_admin";
                string adminPassword = config["SeedData:AdminPassword"] ?? "admin";
                string clientName = config["SeedData:ClientName"] ?? "Oleksa";
                string clientPassword = config["SeedData:ClientPassword"] ?? "1234";
                int clientBalance = int.TryParse(config["SeedData:ClientBalance"], out int parsedBalance)
                    ? parsedBalance
                    : 10000;

                dbContext.Users.AddRange(
                    new DbUser { Name = adminName, PasswordHash = PasswordHasher.Hash(adminPassword), Role = "admin" },
                    new DbUser { Name = clientName, PasswordHash = PasswordHasher.Hash(clientPassword), Role = "client", Balance = clientBalance });
            }

            UpgradeLegacyPasswords(dbContext);

            if (!dbContext.HotelState.Any())
            {
                dbContext.HotelState.Add(new DbHotelState { Id = 1, Revenue = 0 });
            }

            dbContext.SaveChanges();
        }

        private static void UpgradeLegacyPasswords(HotelDbContext dbContext)
        {
            List<DbUser> users = dbContext.Users.ToList();
            bool hasChanges = false;

            foreach (DbUser user in users)
            {
                if (PasswordHasher.IsHashed(user.PasswordHash))
                {
                    continue;
                }

                user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
                hasChanges = true;
            }

            if (hasChanges)
            {
                dbContext.SaveChanges();
            }
        }
    }
}
