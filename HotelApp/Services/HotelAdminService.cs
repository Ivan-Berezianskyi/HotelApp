using HotelApp.Interfaces;
using HotelApp.Models;

namespace HotelApp.Services
{
    internal class HotelAdminService : IHotelAdminService
    {
        private readonly Hotel _hotel;

        public HotelAdminService(Hotel hotel)
        {
            _hotel = hotel;
        }

        public bool TryGetRevenue(out double revenue, out string? errorMessage)
        {
            errorMessage = null;

            revenue = _hotel.Revenue;
            return true;
        }

        public bool TryAddRoom(Room room, out string? errorMessage)
        {
            return _hotel.AddRoom(room, out errorMessage);
        }

        public bool TryRemoveRoom(int number, out string? errorMessage)
        {
            return _hotel.RemoveRoom(number, out errorMessage);
        }
    }
}