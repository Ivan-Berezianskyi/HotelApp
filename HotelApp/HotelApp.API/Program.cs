using HotelApp.Bootstrap;
using HotelApp.Data;
using HotelApp.Interfaces;
using HotelApp.Models;
using HotelApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HotelApp.API.Services.ICurrentUserService, HotelApp.API.Services.CurrentUserService>();

builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("HotelDb")
        ?? throw new InvalidOperationException("Connection string 'HotelDb' not found.")));

builder.Services.AddScoped<Hotel>();
builder.Services.AddScoped<IAccountLoader, AccountLoader>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelAdminService, HotelAdminService>();
builder.Services.AddScoped<Func<IClient, IHotelClientService>>(sp =>
    client => new HotelClientService(
        sp.GetRequiredService<Hotel>(),
        client,
        sp.GetRequiredService<HotelDbContext>()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    DatabaseBootstrapper.EnsureCreatedAndSeed(dbContext, app.Configuration);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
