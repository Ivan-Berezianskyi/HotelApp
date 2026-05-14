using HotelApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Data
{
    internal class HotelDbContext : DbContext
    {
        public DbSet<DbUser> Users => Set<DbUser>();
        public DbSet<DbRoomType> RoomTypes => Set<DbRoomType>();
        public DbSet<DbRoom> Rooms => Set<DbRoom>();
        public DbSet<DbHotelState> HotelState => Set<DbHotelState>();
        public DbSet<DbBooking> Bookings => Set<DbBooking>();

        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(user => user.Id);
                entity.Property(user => user.Name).IsRequired().HasMaxLength(100);
                entity.Property(user => user.PasswordHash).IsRequired().HasMaxLength(200);
                entity.Property(user => user.Role).IsRequired().HasMaxLength(20);
                entity.HasIndex(user => user.Name).IsUnique();
            });

            modelBuilder.Entity<DbRoomType>(entity =>
            {
                entity.ToTable("RoomTypes");
                entity.HasKey(roomType => roomType.Id);
                entity.Property(roomType => roomType.Code).IsRequired().HasMaxLength(20);
                entity.Property(roomType => roomType.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(roomType => roomType.Code).IsUnique();
            });

            modelBuilder.Entity<DbRoom>(entity =>
            {
                entity.ToTable("Rooms");
                entity.HasKey(room => room.Id);
                entity.Property(room => room.Number).IsRequired();
                entity.Property(room => room.RoomTypeCode).IsRequired().HasMaxLength(20);
                entity.Property(room => room.Price).IsRequired();
                entity.Property(room => room.IsOccupied).IsRequired();
                entity.HasIndex(room => room.Number).IsUnique();
            });

            modelBuilder.Entity<DbHotelState>(entity =>
            {
                entity.ToTable("HotelState");
                entity.HasKey(state => state.Id);
                entity.Property(state => state.Revenue).IsRequired();
            });

            modelBuilder.Entity<DbBooking>(entity =>
            {
                entity.ToTable("Bookings");
                entity.HasKey(booking => booking.Id);
                entity.Property(booking => booking.UserId).IsRequired();
                entity.Property(booking => booking.RoomNumber).IsRequired();
                entity.Property(booking => booking.IsActive).IsRequired();
                entity.Property(booking => booking.CreatedUtc).IsRequired();
                entity.HasIndex(booking => new { booking.UserId, booking.RoomNumber, booking.IsActive });
            });
        }
    }
}