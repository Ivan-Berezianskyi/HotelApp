using HotelApp.API.DTOs;
using HotelApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelFacade _hotelFacade;

        public HotelsController(IHotelFacade hotelFacade)
        {
            _hotelFacade = hotelFacade;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RoomDto>> GetAll()
        {
            var rooms = _hotelFacade.GetAllRooms()
                .Select(r => new RoomDto(r.Number, r.Price, r.IsOccupied, r.GetType().Name));
            return Ok(rooms);
        }

        [HttpGet("{number}")]
        public ActionResult<RoomDto> Get(int number)
        {
            var room = _hotelFacade.GetRoom(number);
            if (room == null) return NotFound();
            return Ok(new RoomDto(room.Number, room.Price, room.IsOccupied, room.GetType().Name));
        }
    }
}
