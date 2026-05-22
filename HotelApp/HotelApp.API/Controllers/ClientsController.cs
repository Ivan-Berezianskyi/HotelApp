using HotelApp.API.DTOs;
using HotelApp.API.Models;
using HotelApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IHotelFacade _hotelFacade;
        private readonly HotelApp.API.Services.ICurrentUserService _currentUserService;

        public ClientsController(IHotelFacade hotelFacade, HotelApp.API.Services.ICurrentUserService currentUserService)
        {
            _hotelFacade = hotelFacade;
            _currentUserService = currentUserService;
        }

        [HttpGet("{name}")]
        public ActionResult<ClientInfoDto> GetClient(string name)
        {
            var client = new ApiClient(name, 0);
            double balance = _hotelFacade.GetClientBalance(client);
            return Ok(new ClientInfoDto(name, balance));
        }

        [HttpGet("{name}/orders")]
        public ActionResult<IEnumerable<RoomDto>> GetOrders(string name)
        {
            var client = new ApiClient(name, 0);
            var rooms = _hotelFacade.GetClientOrders(client)
                .Select(r => new RoomDto(r.Number, r.Price, r.IsOccupied, r.GetType().Name));

            return Ok(rooms);
        }

        [HttpPost("{name}/bookings")]
        public ActionResult<OperationResultDto> BookRoom(string name, [FromBody] BookRoomRequest request)
        {
            var client = new ApiClient(name, 0);

            if (!_hotelFacade.TryBookRoom(client, request.RoomNumber, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Booking failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }

        [HttpPost("{name}/payments")]
        public ActionResult<PayRoomResponse> PayForRoom(string name, [FromBody] PayRoomRequest request)
        {
            var client = new ApiClient(name, 0);

            if (!_hotelFacade.TryPayForRoom(client, request.RoomNumber, request.StayDays, out double paidAmount, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Payment failed"));
            }

            double balance = _hotelFacade.GetClientBalance(client);
            return Ok(new PayRoomResponse(paidAmount, balance));
        }

        
    }
}
