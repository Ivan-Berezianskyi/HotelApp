using HotelApp.Data.Entities;
using HotelApp.Security;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Data
{
    internal static class SqliteBootstrapper
    {
        public static void EnsureCreatedAndSeed(HotelDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            EnsureSchemaForExistingDb(dbContext);

            if (!dbContext.RoomTypes.Any())
            {
                dbContext.RoomTypes.AddRange(
                    new DbRoomType { Code = "1", Name = "Standard" },
                    new DbRoomType { Code = "2", Name = "VIP" });
            }

            if (!dbContext.Users.Any())
            {
                dbContext.Users.AddRange(
                    new DbUser { Name = "Головний Адмін", PasswordHash = PasswordHasher.Hash("admin"), Role = "admin" },
                    new DbUser { Name = "Олександр", PasswordHash = PasswordHasher.Hash("1234"), Role = "client", Balance = 10000 });
            }

            UpgradeLegacyPasswords(dbContext);

            if (!dbContext.HotelState.Any())
            {
                dbContext.HotelState.Add(new DbHotelState { Id = 1, Revenue = 0 });
            }

            dbContext.SaveChanges();
        }

        private static void EnsureSchemaForExistingDb(HotelDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS HotelState (
                    Id INTEGER NOT NULL CONSTRAINT PK_HotelState PRIMARY KEY,
                    Revenue REAL NOT NULL
                );
            ");

            dbContext.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS Rooms (
                    Id INTEGER NOT NULL CONSTRAINT PK_Rooms PRIMARY KEY AUTOINCREMENT,
                    Number INTEGER NOT NULL,
                    RoomTypeCode TEXT NOT NULL,
                    Price REAL NOT NULL,
                    IsOccupied INTEGER NOT NULL
                );
            ");

            dbContext.Database.ExecuteSqlRaw(@"
                CREATE UNIQUE INDEX IF NOT EXISTS IX_Rooms_Number ON Rooms (Number);
            ");

            dbContext.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS Bookings (
                    Id INTEGER NOT NULL CONSTRAINT PK_Bookings PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    RoomNumber INTEGER NOT NULL,
                    IsActive INTEGER NOT NULL,
                    StayDays INTEGER NULL,
                    PaidAmount REAL NULL,
                    CreatedUtc TEXT NOT NULL,
                    PaidUtc TEXT NULL
                );
            ");

            dbContext.Database.ExecuteSqlRaw(@"
                CREATE INDEX IF NOT EXISTS IX_Bookings_UserId_RoomNumber_IsActive
                ON Bookings (UserId, RoomNumber, IsActive);
            ");
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