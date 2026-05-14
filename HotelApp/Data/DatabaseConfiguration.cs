using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelApp.Data
{
    internal static class DatabaseConfiguration
    {
        public static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }

        public static DbContextOptions<HotelDbContext> CreateOptions(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("HotelDb")
                ?? throw new InvalidOperationException("Connection string 'HotelDb' not found in appsettings.json.");

            var builder = new DbContextOptionsBuilder<HotelDbContext>();
            builder.UseNpgsql(connectionString);
            return builder.Options;
        }
    }
}