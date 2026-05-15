using HotelApp.Data;
using HotelApp.Data.Entities;
using HotelApp.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Tests.TestSupport;

internal sealed class HotelTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public HotelDbContext DbContext { get; }

    public Hotel Hotel { get; }

    private HotelTestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        DbContextOptions<HotelDbContext> options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = new HotelDbContext(options);
        DbContext.Database.EnsureCreated();
        Hotel = new Hotel(DbContext);
    }

    public static HotelTestDatabase Create() => new HotelTestDatabase();

    public void SeedUsers(params DbUser[] users)
    {
        DbContext.Users.AddRange(users);
        DbContext.SaveChanges();
    }

    public void SeedRooms(params DbRoom[] rooms)
    {
        DbContext.Rooms.AddRange(rooms);
        DbContext.SaveChanges();
    }

    public void SeedBookings(params DbBooking[] bookings)
    {
        DbContext.Bookings.AddRange(bookings);
        DbContext.SaveChanges();
    }

    public void SeedRevenue(double revenue)
    {
        DbContext.HotelState.Add(new DbHotelState { Id = 1, Revenue = revenue });
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _connection.Dispose();
    }
}