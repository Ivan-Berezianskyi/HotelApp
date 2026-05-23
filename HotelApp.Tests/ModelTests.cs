using HotelApp.Interfaces;
using HotelApp.Models;
using HotelApp.Services;
using Xunit;

namespace HotelApp.Tests;

public class ModelTests
{
    [Fact]
    public void Account_CheckPassword_ReturnsTrueForLegacyPlainTextPassword()
    {
        // Arrange
        Admin admin = new Admin("admin", "secret");

        // Act
        bool isValid = admin.CheckPassword("secret");

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Admin_ChangePassword_UpdatesPasswordAndRejectsOldValue()
    {
        // Arrange
        Admin admin = new Admin("admin", "secret");

        // Act
        bool changed = admin.ChangePassword("secret", "newsecret");

        // Assert
        Assert.True(changed);
        Assert.False(admin.CheckPassword("secret"));
        Assert.True(admin.CheckPassword("newsecret"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void Admin_ChangePassword_ReturnsFalseForInvalidNewPassword(int passwordLength)
    {
        // Arrange
        Admin admin = new Admin("admin", "secret");
        string newPassword = new string('a', passwordLength);

        // Act
        bool changed = admin.ChangePassword("secret", newPassword);

        // Assert
        Assert.False(changed);
        Assert.True(admin.CheckPassword("secret"));
    }

    [Fact]
    public void Client_SyncMoney_UpdatesMoneyBalance()
    {
        // Arrange
        Client client = new Client("client", "secret", 100);

        // Act
        client.SyncMoney(42.5);

        // Assert
        Assert.Equal(42.5, client.Money);
    }

    [Fact]
    public void StandardRoom_CalculateCost_UsesBasePriceAndServicesCost()
    {
        // Arrange
        StandardRoom room = new StandardRoom(101, 100);

        // Act
        double cost = room.CalculateCost(3);

        // Assert
        Assert.Equal(300, cost);
    }

    [Fact]
    public void VIPRoom_CalculateCost_UsesMultiplier()
    {
        // Arrange
        VIPRoom room = new VIPRoom(202, 100);

        // Act
        double cost = room.CalculateCost(2);

        // Assert
        Assert.Equal(300, cost);
    }

    [Fact]
    public void Room_BookAndDeBook_TogglesOccupiedState()
    {
        // Arrange
        Room room = new StandardRoom(101, 100);

        // Act
        room.Book();
        bool afterBook = room.IsOccupied;
        room.DeBook();

        // Assert
        Assert.True(afterBook);
        Assert.False(room.IsOccupied);
    }

    [Fact]
    public void AccountStrategyRegistry_ReturnsStrategiesForRoleIds()
    {
        // Arrange
        Admin admin = new Admin("admin", "secret");
        Client client = new Client("client", "secret", 0);

        // Act
        IAccountStrategy? adminStrategy = AccountStrategyRegistry.GetStrategyByRoleId(1);
        IAccountStrategy? clientStrategy = AccountStrategyRegistry.GetStrategyByRoleId(2);
        IAccountStrategy? resolvedAdminStrategy = AccountStrategyRegistry.GetStrategy(admin);
        IAccountStrategy? resolvedClientStrategy = AccountStrategyRegistry.GetStrategy(client);

        // Assert
        Assert.NotNull(adminStrategy);
        Assert.NotNull(clientStrategy);
        Assert.Same(adminStrategy, resolvedAdminStrategy);
        Assert.Same(clientStrategy, resolvedClientStrategy);
        Assert.Equal(2, AccountStrategyRegistry.GetAllStrategies().Count);
    }

    [Fact]
    public void AdminAccountStrategy_CastsAdminAccount()
    {
        // Arrange
        Admin admin = new Admin("admin", "secret");
        AdminAccountStrategy strategy = new AdminAccountStrategy();

        // Act
        bool isApplicable = strategy.IsApplicable(admin);

        // Assert
        Assert.True(isApplicable);
    }

    [Fact]
    public void ClientAccountStrategy_CastsClientAccount()
    {
        // Arrange
        Client client = new Client("client", "secret", 10);
        ClientAccountStrategy strategy = new ClientAccountStrategy();

        // Act
        bool isApplicable = strategy.IsApplicable(client);

        // Assert
        Assert.True(isApplicable);
    }

    [Fact]
    public void RoomCreatorRegistry_ResolvesCreatorsAndCodes()
    {
        // Arrange
        StandardRoom standardRoom = new StandardRoom(1, 100);
        VIPRoom vipRoom = new VIPRoom(2, 200);

        // Act
        bool hasStandard = RoomCreatorRegistry.TryGetCreator("1", out RoomCreator? standardCreator);
        bool hasVip = RoomCreatorRegistry.TryGetCreator("2", out RoomCreator? vipCreator);
        bool hasUnknown = RoomCreatorRegistry.TryGetCreator("9", out RoomCreator? unknownCreator);
        bool standardCodeResolved = RoomCreatorRegistry.TryGetCode(standardRoom, out string? standardCode);
        bool vipCodeResolved = RoomCreatorRegistry.TryGetCode(vipRoom, out string? vipCode);

        // Assert
        Assert.True(hasStandard);
        Assert.True(hasVip);
        Assert.False(hasUnknown);
        Assert.NotNull(standardCreator);
        Assert.NotNull(vipCreator);
        Assert.Null(unknownCreator);
        Assert.Equal("1", standardCode);
        Assert.Equal("2", vipCode);
        Assert.True(standardCodeResolved);
        Assert.True(vipCodeResolved);
    }
}