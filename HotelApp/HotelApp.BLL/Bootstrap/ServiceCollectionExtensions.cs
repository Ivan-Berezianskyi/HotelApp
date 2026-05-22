using HotelApp.Data;
using HotelApp.Interfaces;
using HotelApp.Models;
using HotelApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHotelAppServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = DatabaseConfiguration.ResolveConnectionString(config);

            services.AddDbContext<HotelDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<Hotel>();
            services.AddScoped<IAccountLoader, AccountLoader>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHotelAdminService, HotelAdminService>();
            services.AddScoped<HotelFacade>();
            services.AddScoped<IHotelFacade>(sp => new LoggingHotelFacadeDecorator(
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<LoggingHotelFacadeDecorator>>(),
                sp.GetRequiredService<HotelFacade>()));
            services.AddScoped<IRoomFactory, RoomFactory>();
            services.AddScoped<Func<IClient, IHotelClientService>>(sp =>
                client => new HotelClientService(
                    sp.GetRequiredService<Hotel>(),
                    client,
                    sp.GetRequiredService<HotelDbContext>()));

            return services;
        }
    }
}
