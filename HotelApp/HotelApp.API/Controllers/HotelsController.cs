using HotelApp.Models;
using HotelApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly Hotel _hotel;

        public HotelsController(Hotel hotel)
        {
            _hotel = hotel;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RoomDto>> GetAll()
        {
            var rooms = _hotel.Rooms.Select(r => new RoomDto(r.Number, r.Price, r.IsOccupied, r.GetType().Name));
            return Ok(rooms);
        }

        [HttpGet("{number}")]
        public ActionResult<RoomDto> Get(int number)
        {
            var room = _hotel.FindRoomByNumber(number);
            if (room == null) return NotFound();
            return Ok(new RoomDto(room.Number, room.Price, room.IsOccupied, room.GetType().Name));
        }
    }
}
