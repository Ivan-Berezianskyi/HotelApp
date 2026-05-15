using HotelApp.Data.Entities;
using HotelApp.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelApp.Tests;

public class DataTests
{
    [Fact]
    public void HotelDbContext_MapsExpectedTablesAndIndexes()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        var model = database.DbContext.Model;
        var userType = model.FindEntityType(typeof(DbUser));
        var bookingType = model.FindEntityType(typeof(DbBooking));
        var roomType = model.FindEntityType(typeof(DbRoom));

        // Assert
        Assert.NotNull(userType);
        Assert.NotNull(bookingType);
        Assert.NotNull(roomType);
        Assert.Equal("Users", userType!.GetTableName());
        Assert.Equal("Bookings", bookingType!.GetTableName());
        Assert.Equal("Rooms", roomType!.GetTableName());
        Assert.Contains(userType.GetIndexes(), index => index.IsUnique && index.Properties.Single().Name == nameof(DbUser.Name));
        Assert.Contains(roomType.GetIndexes(), index => index.IsUnique && index.Properties.Single().Name == nameof(DbRoom.Number));
        Assert.Contains(bookingType.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(DbUser));
        Assert.Contains(bookingType.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(DbRoom));
    }
}