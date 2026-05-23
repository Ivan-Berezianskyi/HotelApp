using HotelApp.Interfaces;
using HotelApp.Models;
using Microsoft.Extensions.Logging;

namespace HotelApp.Services
{
    public class LoggingHotelFacadeDecorator : IHotelFacade
    {
        private readonly ILogger<LoggingHotelFacadeDecorator> _logger;
        private readonly HotelFacade _inner;

        public LoggingHotelFacadeDecorator(ILogger<LoggingHotelFacadeDecorator> logger, HotelFacade inner)
        {
            _logger = logger;
            _inner = inner;
        }

        public IAccount? Authenticate(int roleId, string name, string password)
        {
            _logger.LogInformation("Authenticate roleId={RoleId} name={Name}", roleId, name);
            return _inner.Authenticate(roleId, name, password);
        }

        public IReadOnlyList<Room> GetAllRooms()
        {
            _logger.LogInformation("GetAllRooms");
            return _inner.GetAllRooms();
        }

        public Room? GetRoom(int number)
        {
            _logger.LogInformation("GetRoom number={Number}", number);
            return _inner.GetRoom(number);
        }

        public bool TryGetRevenue(out double revenue, out string? errorMessage)
        {
            _logger.LogInformation("TryGetRevenue");
            bool result = _inner.TryGetRevenue(out revenue, out errorMessage);
            if (!result)
            {
                _logger.LogWarning("TryGetRevenue failed: {ErrorMessage}", errorMessage);
            }

            return result;
        }

        public bool TryAddRoom(string typeCode, int number, double price, out string? errorMessage)
        {
            _logger.LogInformation("TryAddRoom typeCode={TypeCode} number={Number} price={Price}", typeCode, number, price);
            bool result = _inner.TryAddRoom(typeCode, number, price, out errorMessage);
            if (!result)
            {
                _logger.LogWarning("TryAddRoom failed: {ErrorMessage}", errorMessage);
            }

            return result;
        }

        public bool TryRemoveRoom(int number, out string? errorMessage)
        {
            _logger.LogInformation("TryRemoveRoom number={Number}", number);
            bool result = _inner.TryRemoveRoom(number, out errorMessage);
            if (!result)
            {
                _logger.LogWarning("TryRemoveRoom failed: {ErrorMessage}", errorMessage);
            }

            return result;
        }

        public bool TryChangePasswordByName(string name, string currentPassword, string newPassword, out string? errorMessage)
        {
            _logger.LogInformation("TryChangePasswordByName name={Name}", name);
            bool result = _inner.TryChangePasswordByName(name, currentPassword, newPassword, out errorMessage);
            if (!result)
            {
                _logger.LogWarning("TryChangePasswordByName failed: {ErrorMessage}", errorMessage);
            }

            return result;
        }

        public double GetClientBalance(IClient client)
        {
            _logger.LogInformation("GetClientBalance name={Name}", client.Name);
            return _inner.GetClientBalance(client);
        }

        public IReadOnlyList<Room> GetClientOrders(IClient client)
        {
            _logger.LogInformation("GetClientOrders name={Name}", client.Name);
            return _inner.GetClientOrders(client);
        }

        public bool TryBookRoom(IClient client, int roomNumber, out string? errorMessage)
        {
            _logger.LogInformation("TryBookRoom name={Name} roomNumber={RoomNumber}", client.Name, roomNumber);
            bool result = _inner.TryBookRoom(client, roomNumber, out errorMessage);
            if (!result)
            {
                _logger.LogWarning("TryBookRoom failed: {ErrorMessage}", errorMessage);
            }

            return result;
        }

        public bool TryPayForRoom(IClient client, int roomNumber, int stayDays, out double paidAmount, out string? errorMessage)
        {
            _logger.LogInformation("TryPayForRoom name={Name} roomNumber={RoomNumber} stayDays={StayDays}", client.Name, roomNumber, stayDays);
            bool result = _inner.TryPayForRoom(client, roomNumber, stayDays, out paidAmount, out errorMessage);
            if (!result)
            {
                _logger.LogWarning("TryPayForRoom failed: {ErrorMessage}", errorMessage);
            }

            return result;
        }
    }
}
