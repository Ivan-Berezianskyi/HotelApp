using System.Collections.ObjectModel;
using HotelApp.Interfaces;

namespace HotelApp.Models
{
    internal class Client : Account, IClient
    {
        public double Money { get; private set; }
        private readonly List<Room> _myOrders = new List<Room>();

        public Client(string name, string password, double money) : base(name, password)
        {
            Money = money;
        }

        public bool Order(Room room)
        {
            if (room == null)
            {
                return false;
            }

            if (room.IsOccupied)
            {
                return false;
            }

            if (_myOrders.Contains(room))
            {
                return false;
            }
            
            room.Book();
            _myOrders.Add(room);
            
            return true;
        }

        public IReadOnlyList<Room> GetMyOrders()
        {
            return new ReadOnlyCollection<Room>(_myOrders);
        }

        public bool Pay(Room room, int stayDays, out double paidAmount)
        {
            paidAmount = 0;

            if (room == null)
            {
                return false;
            }

            if (stayDays <= 0 || !_myOrders.Contains(room))
            {
                return false;
            }

            double cost = room.CalculateCost(stayDays);

            if (Money < cost)
            {
                return false;
            }

            Money -= cost;
            room.DeBook();
            _myOrders.Remove(room);
            paidAmount = cost;
            
            return true;
        }
    }
}
