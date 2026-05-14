using HotelApp.Models;
using HotelApp.Services;
using HotelApp.UI;
using HotelApp.Interfaces;
using HotelApp.Infrastructure;
using HotelApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var config = DatabaseConfiguration.BuildConfiguration();
            var services = ConfigureServices(config);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<HotelDbContext>();
                DatabaseBootstrapper.EnsureCreatedAndSeed(dbContext, config);

                var runner = new ApplicationRunner(serviceProvider);
                runner.Run();
            }
        }

        static IServiceCollection ConfigureServices(IConfiguration config)
        {
            var services = new ServiceCollection();

            // Configuration
            services.AddSingleton(config);

            // Database
            services.AddDbContext<HotelDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("HotelDb")
                    ?? throw new InvalidOperationException("Connection string 'HotelDb' not found.")));

            // Models
            services.AddScoped<Hotel>();

            // Infrastructure
            services.AddSingleton<HotelApp.Interfaces.ILogger>(sp =>
            {
                var logger = new LoggerComposite();

                if (bool.TryParse(config["Logger:FileProvider"], out bool useFileLogger) && useFileLogger)
                {
                    string logPath = config["Logger:FilePath"] ?? "hotel.log";
                    logger.AddLogger(new FileLogger(logPath));
                }

                if (bool.TryParse(config["Logger:ConsoleProvider"], out bool useConsoleLogger) && useConsoleLogger)
                {
                    logger.AddLogger(new ConsoleLogger());
                }

                return logger;
            });

            // Account loading
            services.AddScoped<IAccountLoader, AccountLoader>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHotelAdminService, HotelAdminService>();
            services.AddScoped<Func<IClient, IHotelClientService>>(sp =>
                client => new HotelClientService(
                    sp.GetRequiredService<Hotel>(),
                    client,
                    sp.GetRequiredService<HotelDbContext>()));

            // UI
            services.AddScoped<IAccountMenuFactory, AccountMenuFactory>();
            services.AddScoped<ILoginMenu, LoginMenu>();

            return services;
        }
    }
}