using HotelApp.UI;
using HotelApp.Interfaces;
using HotelApp.Infrastructure;
using HotelApp.UI.Api;
using HotelApp.UI.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingSystem
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
            var services = ConfigureServices(config);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var runner = new ApplicationRunner(serviceProvider);
                runner.Run();
            }
        }

        static IServiceCollection ConfigureServices(IConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddSingleton(config);

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

            string? apiBaseUrl = config["Api:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                throw new InvalidOperationException("Api:BaseUrl is not configured in appsettings.json.");
            }

            services.AddHttpClient<IHotelApiClient, HotelApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            services.AddScoped<IAccountMenuFactory, AccountMenuFactory>();
            services.AddScoped<ILoginMenu, LoginMenu>();

            return services;
        }
    }
}