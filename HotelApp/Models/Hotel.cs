namespace HotelApp.Models
{
    internal class Hotel
    {
        private readonly List<Room> _rooms = new List<Room>();
        public IReadOnlyList<Room> Rooms => _rooms.AsReadOnly();
        public double Revenue { get; private set; } = 0;
        public void AddRevenue(double amount) => Revenue += amount;

        public Room? FindRoomByNumber(int roomNumber)
        {
            return _rooms.Find(r => r.Number == roomNumber);
        }

        public bool AddRoom(Room room, out string? errorMessage)
        {
            if (room == null)
            {
                errorMessage = "Кімната не передана.";
                return false;
            }

            if (_rooms.Exists(r => r.Number == room.Number))
            {
                errorMessage = "Кімната з таким номером вже існує.";
                return false;
            }

            _rooms.Add(room);
            errorMessage = null;

            return true;
        }

        public bool RemoveRoom(int number, out string? errorMessage)
        {
            Room? room = _rooms.Find(r => r.Number == number);
            if (room == null)
            {
                errorMessage = $"Кімната {number} не знайдена.";
                return false;
            }

            if (room.IsOccupied)
            {
                errorMessage = "Не можна видалити кімнату: вона зайнята.";
                return false;
            }

            _rooms.Remove(room);
            errorMessage = null;

            return true;
        }
    }
}
