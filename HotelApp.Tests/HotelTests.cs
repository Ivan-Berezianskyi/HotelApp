using HotelApp.Data.Entities;
using HotelApp.Models;
using HotelApp.Tests.TestSupport;
using Xunit;

namespace HotelApp.Tests;

public class HotelTests
{
    [Fact]
    public void Revenue_ReturnsZeroWhenStateIsMissing()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        double revenue = database.Hotel.Revenue;

        // Assert
        Assert.Equal(0, revenue);
    }

    [Fact]
    public void AddRevenue_CreatesStateAndAddsAmount()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        database.Hotel.AddRevenue(150);

        // Assert
        Assert.Equal(150, database.Hotel.Revenue);
    }

    [Fact]
    public void AddRevenue_IgnoresNonPositiveAmounts()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        database.Hotel.AddRevenue(-10);

        // Assert
        Assert.Equal(0, database.Hotel.Revenue);
    }

    [Fact]
    public void AddRoom_PersistsRoomWhenTypeIsKnown()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        Room room = new StandardRoom(101, 200);

        // Act
        bool added = database.Hotel.AddRoom(room, out string? errorMessage);

        // Assert
        Assert.True(added);
        Assert.Null(errorMessage);
        Assert.Single(database.Hotel.Rooms);
        Assert.Equal(101, database.Hotel.Rooms[0].Number);
    }

    [Fact]
    public void AddRoom_ReturnsFalseForDuplicateNumber()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedRooms(new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = false });

        // Act
        bool added = database.Hotel.AddRoom(new StandardRoom(101, 200), out string? errorMessage);

        // Assert
        Assert.False(added);
        Assert.Equal("Кімната з таким номером вже існує.", errorMessage);
    }

    [Fact]
    public void AddRoom_ReturnsFalseForUnknownRoomType()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        bool added = database.Hotel.AddRoom(new UnknownRoom(999, 100), out string? errorMessage);

        // Assert
        Assert.False(added);
        Assert.Equal("Невідомий тип кімнати.", errorMessage);
    }

    [Fact]
    public void AddRoom_ReturnsFalseWhenRoomIsNull()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        bool added = database.Hotel.AddRoom(null!, out string? errorMessage);

        // Assert
        Assert.False(added);
        Assert.Equal("Кімната не передана.", errorMessage);
    }

    [Fact]
    public void Rooms_SkipsUnknownRoomTypes()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedRooms(
            new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = false },
            new DbRoom { Number = 202, RoomTypeCode = "999", Price = 200, IsOccupied = false });

        // Act
        IReadOnlyList<Room> rooms = database.Hotel.Rooms;

        // Assert
        Assert.Single(rooms);
        Assert.Equal(101, rooms[0].Number);
    }

    [Fact]
    public void FindRoomByNumber_ReturnsMappedRoom()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedRooms(new DbRoom { Number = 202, RoomTypeCode = "2", Price = 150, IsOccupied = true });

        // Act
        Room? room = database.Hotel.FindRoomByNumber(202);

        // Assert
        Assert.NotNull(room);
        Assert.IsType<VIPRoom>(room);
        Assert.True(room!.IsOccupied);
    }

    [Fact]
    public void FindRoomByNumber_ReturnsNullWhenRoomMissing()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        Room? room = database.Hotel.FindRoomByNumber(999);

        // Assert
        Assert.Null(room);
    }

    [Fact]
    public void RemoveRoom_DeletesFreeRoom()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedRooms(new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = false });

        // Act
        bool removed = database.Hotel.RemoveRoom(101, out string? errorMessage);

        // Assert
        Assert.True(removed);
        Assert.Null(errorMessage);
        Assert.Empty(database.Hotel.Rooms);
    }

    [Fact]
    public void RemoveRoom_FailsWhenRoomIsOccupied()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedRooms(new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = true });

        // Act
        bool removed = database.Hotel.RemoveRoom(101, out string? errorMessage);

        // Assert
        Assert.False(removed);
        Assert.Equal("Не можна видалити кімнату: вона зайнята.", errorMessage);
    }

    [Fact]
    public void RemoveRoom_ReturnsFalseWhenRoomMissing()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        bool removed = database.Hotel.RemoveRoom(404, out string? errorMessage);

        // Assert
        Assert.False(removed);
        Assert.Equal("Кімната 404 не знайдена.", errorMessage);
    }

    [Fact]
    public void TrySetRoomOccupied_UpdatesOccupancy()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();
        database.SeedRooms(new DbRoom { Number = 101, RoomTypeCode = "1", Price = 100, IsOccupied = false });

        // Act
        bool updated = database.Hotel.TrySetRoomOccupied(101, true, out string? errorMessage);

        // Assert
        Assert.True(updated);
        Assert.Null(errorMessage);
        Assert.True(database.Hotel.FindRoomByNumber(101)!.IsOccupied);
    }

    [Fact]
    public void TrySetRoomOccupied_ReturnsFalseWhenRoomMissing()
    {
        // Arrange
        using HotelTestDatabase database = HotelTestDatabase.Create();

        // Act
        bool updated = database.Hotel.TrySetRoomOccupied(404, true, out string? errorMessage);

        // Assert
        Assert.False(updated);
        Assert.Equal("Кімната 404 не знайдена.", errorMessage);
    }

    private sealed class UnknownRoom : Room
    {
        public UnknownRoom(int number, double price) : base(number, price)
        {
        }

        public override double CalculateCost(int days) => Price * days;
    }
}