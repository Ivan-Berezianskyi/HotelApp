using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    public class HotelFacade : IHotelFacade
    {
        private readonly IAuthService _authService;
        private readonly IHotelAdminService _adminService;
        private readonly Hotel _hotel;
        private readonly IRoomFactory _roomFactory;
        private readonly Func<IClient, IHotelClientService> _clientServiceFactory;

        public HotelFacade(
            IAuthService authService,
            IHotelAdminService adminService,
            Hotel hotel,
            IRoomFactory roomFactory,
            Func<IClient, IHotelClientService> clientServiceFactory)
        {
            _authService = authService;
            _adminService = adminService;
            _hotel = hotel;
            _roomFactory = roomFactory;
            _clientServiceFactory = clientServiceFactory;
        }

        public IAccount? Authenticate(int roleId, string name, string password)
        {
            return _authService.Authenticate(roleId, name, password);
        }

        public IReadOnlyList<Room> GetAllRooms()
        {
            return _hotel.Rooms;
        }

        public Room? GetRoom(int number)
        {
            return _hotel.FindRoomByNumber(number);
        }

        public bool TryGetRevenue(out double revenue, out string? errorMessage)
        {
            return _adminService.TryGetRevenue(out revenue, out errorMessage);
        }

        public bool TryAddRoom(string typeCode, int number, double price, out string? errorMessage)
        {
            if (!_roomFactory.TryCreate(typeCode, number, price, out Room? room, out errorMessage) || room == null)
            {
                return false;
            }

            return _adminService.TryAddRoom(room, out errorMessage);
        }

        public bool TryRemoveRoom(int number, out string? errorMessage)
        {
            return _adminService.TryRemoveRoom(number, out errorMessage);
        }

        public bool TryChangePasswordByName(string name, string currentPassword, string newPassword, out string? errorMessage)
        {
            return _adminService.TryChangePasswordByName(name, currentPassword, newPassword, out errorMessage);
        }

        public double GetClientBalance(IClient client)
        {
            return GetClientService(client).GetBalance();
        }

        public IReadOnlyList<Room> GetClientOrders(IClient client)
        {
            return GetClientService(client).GetMyOrders();
        }

        public bool TryBookRoom(IClient client, int roomNumber, out string? errorMessage)
        {
            return GetClientService(client).TryBookRoom(roomNumber, out errorMessage);
        }

        public bool TryPayForRoom(IClient client, int roomNumber, int stayDays, out double paidAmount, out string? errorMessage)
        {
            return GetClientService(client).TryPayForRoom(roomNumber, stayDays, out paidAmount, out errorMessage);
        }

        private IHotelClientService GetClientService(IClient client)
        {
            return _clientServiceFactory(client);
        }
    }
}
