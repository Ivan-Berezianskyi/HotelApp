using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelApp.Data
{
    internal class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
    {
        public HotelDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<HotelDbContext> builder = new DbContextOptionsBuilder<HotelDbContext>();
            builder.UseSqlite("Data Source=hotelapp.db");

            return new HotelDbContext(builder.Options);
        }
    }
}