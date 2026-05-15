using Microsoft.EntityFrameworkCore.Design;

namespace HotelApp.Data
{
    internal class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
    {
        public HotelDbContext CreateDbContext(string[] args)
        {
            var config = DatabaseConfiguration.BuildConfiguration();
            return new HotelDbContext(DatabaseConfiguration.CreateOptions(config));
        }
    }
}