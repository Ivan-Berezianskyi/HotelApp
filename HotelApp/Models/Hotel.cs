using HotelApp.Data;
using HotelApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Models
{
    internal class Hotel
    {
        private readonly HotelDbContext _dbContext;

        public Hotel(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IReadOnlyList<Room> Rooms
        {
            get
            {
                List<DbRoom> dbRooms = _dbContext.Rooms.AsNoTracking().ToList();
                List<Room> rooms = new List<Room>();

                foreach (DbRoom dbRoom in dbRooms)
                {
                    Room? room = MapDbRoomToDomain(dbRoom);
                    if (room != null)
                    {
                        rooms.Add(room);
                    }
                }

                return rooms.AsReadOnly();
            }
        }

        public double Revenue
        {
            get
            {
                DbHotelState? state = _dbContext.HotelState.AsNoTracking().FirstOrDefault(item => item.Id == 1);
                return state?.Revenue ?? 0;
            }
        }

        public void AddRevenue(double amount)
        {
            if (amount <= 0)
            {
                return;
            }

            DbHotelState state = GetOrCreateHotelState();
            state.Revenue += amount;
            _dbContext.SaveChanges();
        }

        public Room? FindRoomByNumber(int roomNumber)
        {
            DbRoom? dbRoom = _dbContext.Rooms.AsNoTracking().FirstOrDefault(room => room.Number == roomNumber);
            if (dbRoom == null)
            {
                return null;
            }

            return MapDbRoomToDomain(dbRoom);
        }

        public bool AddRoom(Room room, out string? errorMessage)
        {
            if (room == null)
            {
                errorMessage = "Кімната не передана.";
                return false;
            }

            if (_dbContext.Rooms.Any(existingRoom => existingRoom.Number == room.Number))
            {
                errorMessage = "Кімната з таким номером вже існує.";
                return false;
            }

            if (!TryResolveRoomTypeCode(room, out string? roomTypeCode) || string.IsNullOrWhiteSpace(roomTypeCode))
            {
                errorMessage = "Невідомий тип кімнати.";
                return false;
            }

            _dbContext.Rooms.Add(new DbRoom
            {
                Number = room.Number,
                RoomTypeCode = roomTypeCode,
                Price = room.Price,
                IsOccupied = room.IsOccupied
            });

            _dbContext.SaveChanges();
            errorMessage = null;

            return true;
        }

        public bool RemoveRoom(int number, out string? errorMessage)
        {
            DbRoom? dbRoom = _dbContext.Rooms.FirstOrDefault(room => room.Number == number);
            if (dbRoom == null)
            {
                errorMessage = $"Кімната {number} не знайдена.";
                return false;
            }

            if (dbRoom.IsOccupied)
            {
                errorMessage = "Не можна видалити кімнату: вона зайнята.";
                return false;
            }

            _dbContext.Rooms.Remove(dbRoom);
            _dbContext.SaveChanges();
            errorMessage = null;

            return true;
        }

        public bool TrySetRoomOccupied(int number, bool isOccupied, out string? errorMessage)
        {
            DbRoom? dbRoom = _dbContext.Rooms.FirstOrDefault(room => room.Number == number);
            if (dbRoom == null)
            {
                errorMessage = $"Кімната {number} не знайдена.";
                return false;
            }

            dbRoom.IsOccupied = isOccupied;
            _dbContext.SaveChanges();
            errorMessage = null;

            return true;
        }

        private DbHotelState GetOrCreateHotelState()
        {
            DbHotelState? state = _dbContext.HotelState.FirstOrDefault(item => item.Id == 1);
            if (state != null)
            {
                return state;
            }

            state = new DbHotelState { Id = 1, Revenue = 0 };
            _dbContext.HotelState.Add(state);
            return state;
        }

        private static bool TryResolveRoomTypeCode(Room room, out string? code)
        {
            if (room is StandardRoom)
            {
                code = "1";
                return true;
            }

            if (room is VIPRoom)
            {
                code = "2";
                return true;
            }

            code = null;
            return false;
        }

        private static Room? MapDbRoomToDomain(DbRoom dbRoom)
        {
            Room? room = dbRoom.RoomTypeCode switch
            {
                "1" => new StandardRoom(dbRoom.Number, dbRoom.Price),
                "2" => new VIPRoom(dbRoom.Number, dbRoom.Price),
                _ => null
            };

            if (room == null)
            {
                return null;
            }

            if (dbRoom.IsOccupied)
            {
                room.Book();
            }

            return room;
        }
    }
}
