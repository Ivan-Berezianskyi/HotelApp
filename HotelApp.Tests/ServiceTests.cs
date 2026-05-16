using HotelApp.Data.Entities;
using HotelApp.Interfaces;
using HotelApp.Models;
using HotelApp.Services;
using HotelApp.Tests.TestSupport;
using Xunit;

namespace HotelApp.Tests;

public class ServiceTests
{
    [Fact]
    public void AccountLoader_ReturnsOnlySupportedRoles()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedUsers(
            new DbUser { Name = "admin", PasswordHash = "secret", Role = "admin" },
            new DbUser { Name = "client", PasswordHash = "secret", Role = "client", Balance = 75 },
            new DbUser { Name = "ignored", PasswordHash = "secret", Role = "guest" });

        AccountLoader loader = new AccountLoader(database.DbContext);

        // Act
        List<IAccount> accounts = loader.LoadAccountsFromDb();

        // Assert
        Assert.Equal(2, accounts.Count);
        Assert.Contains(accounts, account => account is Admin && account.Name == "admin");
        Assert.Contains(accounts, account => account is Client client && client.Money == 75);
    }

    [Fact]
    public void AuthService_AuthenticatesByRoleNameAndPasswordAndReloadsAccounts()
    {
        // Arrange
        Admin admin = new Admin("Admin", "secret");
        FakeAccountLoader loader = new FakeAccountLoader(new List<IAccount> { admin });
        AuthService service = new AuthService(loader);

        // Act
        IAccount? firstMatch = service.Authenticate(1, " admin ", "secret");
        IAccount? secondMatch = service.Authenticate(1, "ADMIN", "secret");

        // Assert
        Assert.Same(admin, firstMatch);
        Assert.Same(admin, secondMatch);
        Assert.Equal(2, loader.LoadCount);
    }

    [Fact]
    public void AuthService_SeesPasswordChangesOnSubsequentAuthentications()
    {
        // Arrange
        Admin admin = new Admin("Admin", "secret");
        FakeAccountLoader loader = new FakeAccountLoader(new List<IAccount> { admin });
        AuthService service = new AuthService(loader);

        // Act
        IAccount? firstMatch = service.Authenticate(1, "Admin", "secret");
        admin.ChangePassword("secret", "newsecret");
        IAccount? secondMatch = service.Authenticate(1, "Admin", "newsecret");

        // Assert
        Assert.Same(admin, firstMatch);
        Assert.Same(admin, secondMatch);
        Assert.Equal(2, loader.LoadCount);
    }

    [Fact]
    public void AuthService_ReturnsNullForWrongRoleOrPassword()
    {
        // Arrange
        FakeAccountLoader loader = new FakeAccountLoader(new List<IAccount>
        {
            new Admin("Admin", "secret"),
            new Client("Client", "secret", 10)
        });
        AuthService service = new AuthService(loader);

        // Act
        IAccount? wrongRole = service.Authenticate(2, "Admin", "secret");
        IAccount? wrongPassword = service.Authenticate(1, "Admin", "bad");

        // Assert
        Assert.Null(wrongRole);
        Assert.Null(wrongPassword);
    }

    [Fact]
    public void AuthService_ReturnsNullForUnknownRoleId()
    {
        // Arrange
        FakeAccountLoader loader = new FakeAccountLoader(new List<IAccount>
        {
            new Admin("Admin", "secret")
        });
        AuthService service = new AuthService(loader);

        // Act
        IAccount? result = service.Authenticate(99, "Admin", "secret");

        // Assert
        Assert.Null(result);
        Assert.Equal(1, loader.LoadCount);
    }

    [Fact]
    public void HotelAdminService_ReturnsRevenueAndUpdatesRooms()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedRevenue(250);
        database.SeedUsers(new DbUser { Name = "Admin", PasswordHash = "secret", Role = "Admin" });
        HotelAdminService service = new HotelAdminService(database.Hotel, database.DbContext);
        Admin admin = new Admin(" admin ", "secret");

        // Act
        bool revenueOk = service.TryGetRevenue(out double revenue, out string? revenueError);
        bool addRoomOk = service.TryAddRoom(new StandardRoom(101, 100), out string? addRoomError);
        bool removeRoomOk = service.TryRemoveRoom(101, out string? removeRoomError);
        bool passwordChanged = service.TryChangePassword(admin, "secret", "newsecret", out string? changeError);

        // Assert
        Assert.True(revenueOk);
        Assert.Equal(250, revenue);
        Assert.Null(revenueError);
        Assert.True(addRoomOk);
        Assert.Null(addRoomError);
        Assert.True(removeRoomOk);
        Assert.Null(removeRoomError);
        Assert.True(passwordChanged);
        Assert.Null(changeError);
        Assert.True(admin.CheckPassword("newsecret"));
        Assert.NotEqual("secret", database.DbContext.Users.Single(user => user.Name == "Admin").PasswordHash);
    }

    [Fact]
    public void HotelAdminService_ReturnsFalseWhenValidationOrDatabaseLookupFails()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedUsers(new DbUser { Name = "admin", PasswordHash = "secret", Role = "admin" });
        HotelAdminService service = new HotelAdminService(database.Hotel, database.DbContext);
        Admin admin = new Admin("admin", "secret");

        // Act
        bool changedWithWrongPassword = service.TryChangePassword(admin, "wrong", "newsecret", out string? wrongPasswordError);
        bool changedWithShortPassword = service.TryChangePassword(admin, "secret", "123", out string? shortPasswordError);
        database.DbContext.Users.Remove(database.DbContext.Users.Single(user => user.Name == "admin"));
        database.DbContext.SaveChanges();
        bool changedWithoutDbUser = service.TryChangePassword(admin, "secret", "newsecret", out string? missingUserError);

        // Assert
        Assert.False(changedWithWrongPassword);
        Assert.Equal("Помилка: невірний поточний пароль.", wrongPasswordError);
        Assert.False(changedWithShortPassword);
        Assert.Equal("Помилка: новий пароль має містити мінімум 4 символи.", shortPasswordError);
        Assert.False(changedWithoutDbUser);
        Assert.Equal("Помилка: адміністратора не знайдено в БД.", missingUserError);
    }

    [Fact]
    public void HotelClientService_BooksPaysAndListsOrders()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedUsers(new DbUser { Id = 1, Name = "client", PasswordHash = "secret", Role = "client", Balance = 1000 });
        database.SeedRooms(new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = false });
        Client client = new Client("client", "secret", 1000);
        HotelClientService service = new HotelClientService(database.Hotel, client, database.DbContext);

        // Act
        bool booked = service.TryBookRoom(101, out string? bookError);
        IReadOnlyList<Room> activeOrders = service.GetMyOrders();
        bool paid = service.TryPayForRoom(101, 2, out double paidAmount, out string? payError);
        IReadOnlyList<Room> clearedOrders = service.GetMyOrders();

        // Assert
        Assert.True(booked);
        Assert.Null(bookError);
        Assert.Single(activeOrders);
        Assert.True(paid);
        Assert.Null(payError);
        Assert.Equal(200, paidAmount);
        Assert.Equal(800, client.Money);
        Assert.Empty(clearedOrders);
        Assert.False(database.Hotel.FindRoomByNumber(101)!.IsOccupied);
        Assert.Equal(200, database.Hotel.Revenue);
    }

    [Fact]
    public void HotelClientService_HandlesClientRoleRegardlessOfCase()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedUsers(new DbUser { Id = 1, Name = "client", PasswordHash = "secret", Role = "Client", Balance = 1000 });
        database.SeedRooms(new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = false });
        Client client = new Client("CLIENT", "secret", 1000);
        HotelClientService service = new HotelClientService(database.Hotel, client, database.DbContext);

        // Act
        bool booked = service.TryBookRoom(101, out string? bookError);

        // Assert
        Assert.True(booked);
        Assert.Null(bookError);
    }

    [Fact]
    public void HotelClientService_ReturnsErrorsForInvalidBookingAndPaymentScenarios()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedUsers(new DbUser { Id = 1, Name = "client", PasswordHash = "secret", Role = "client", Balance = 50 });
        database.SeedRooms(new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = false });
        database.SeedBookings(new DbBooking { UserId = 1, RoomNumber = 101, IsActive = true, CreatedUtc = DateTime.UtcNow });
        Client client = new Client("client", "secret", 50);
        HotelClientService service = new HotelClientService(database.Hotel, client, database.DbContext);

        // Act
        bool invalidDays = service.TryPayForRoom(101, 0, out double invalidDaysAmount, out string? invalidDaysError);
        bool duplicateBooking = service.TryBookRoom(101, out string? duplicateBookingError);
        bool insufficientFunds = service.TryPayForRoom(101, 2, out double insufficientAmount, out string? insufficientError);

        // Assert
        Assert.False(invalidDays);
        Assert.Equal(0, invalidDaysAmount);
        Assert.Equal("Помилка: кількість днів має бути додатною.", invalidDaysError);
        Assert.False(duplicateBooking);
        Assert.Equal("Помилка: ця кімната вже у ваших активних бронюваннях.", duplicateBookingError);
        Assert.False(insufficientFunds);
        Assert.Equal(0, insufficientAmount);
        Assert.Equal("Помилка: оплата не відбулась.", insufficientError);
    }

    [Fact]
    public void HotelClientService_ReturnsErrorsWhenClientOrRoomIsMissing()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        Client client = new Client("client", "secret", 50);
        HotelClientService service = new HotelClientService(database.Hotel, client, database.DbContext);

        // Act
        bool missingClientBook = service.TryBookRoom(101, out string? missingClientBookError);
        bool missingClientPay = service.TryPayForRoom(101, 1, out double missingClientPayAmount, out string? missingClientPayError);

        database.SeedUsers(new DbUser { Id = 1, Name = "client", PasswordHash = "secret", Role = "client", Balance = 50 });

        bool missingRoomBook = service.TryBookRoom(101, out string? missingRoomBookError);
        bool missingRoomPay = service.TryPayForRoom(101, 1, out double missingRoomPayAmount, out string? missingRoomPayError);

        // Assert
        Assert.False(missingClientBook);
        Assert.Equal("Помилка: клієнта не знайдено в БД.", missingClientBookError);
        Assert.False(missingClientPay);
        Assert.Equal(0, missingClientPayAmount);
        Assert.Equal("Помилка: клієнта не знайдено в БД.", missingClientPayError);
        Assert.False(missingRoomBook);
        Assert.Equal("Помилка: кімнату не знайдено.", missingRoomBookError);
        Assert.False(missingRoomPay);
        Assert.Equal(0, missingRoomPayAmount);
        Assert.Equal("Помилка: такої кімнати немає у ваших бронюваннях.", missingRoomPayError);
    }

    private sealed class FakeAccountLoader : IAccountLoader
    {
        private readonly List<IAccount> _accounts;

        public int LoadCount { get; private set; }

        public FakeAccountLoader(List<IAccount> accounts)
        {
            _accounts = accounts;
        }

        public List<IAccount> LoadAccountsFromDb()
        {
            LoadCount++;
            return _accounts.ToList();
        }
    }
}