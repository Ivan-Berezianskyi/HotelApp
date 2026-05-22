using DotNetEnv;
using HotelApp.Bootstrap;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Env.TraversePath().Load();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .Filter.ByIncludingOnly(logEvent =>
            logEvent.Properties.TryGetValue("SourceContext", out var value)
            && value.ToString() == "\"HotelApp.Services.LoggingHotelFacadeDecorator\"")
        .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HotelApp.API.Services.ICurrentUserService, HotelApp.API.Services.CurrentUserService>();

builder.Services.AddHotelAppServices(builder.Configuration);

var app = builder.Build();

DatabaseBootstrapper.EnsureCreatedAndSeed(app.Services, app.Configuration);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
