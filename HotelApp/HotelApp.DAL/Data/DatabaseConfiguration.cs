using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelApp.Data
{
    public static class DatabaseConfiguration
    {
        public static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }

        public static string ResolveConnectionString(IConfiguration config)
        {
            bool isCloud = config.GetValue<bool>("Mode:IsCloud");

            string? connectionString;
            if (isCloud)
            {
                connectionString = config["DATABASE_URL_CLOUD"];
            }
            else
            {
                connectionString = config["DATABASE_URL_LOCAL"];
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string not found.");
            }

            return connectionString;
        }

        public static DbContextOptions<HotelDbContext> CreateOptions(IConfiguration config)
        {
            var connectionString = ResolveConnectionString(config);
            var builder = new DbContextOptionsBuilder<HotelDbContext>();
            builder.UseNpgsql(connectionString);
            return builder.Options;
        }
    }
}