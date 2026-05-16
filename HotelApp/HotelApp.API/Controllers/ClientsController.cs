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
        private readonly Func<IClient, IHotelClientService> _clientServiceFactory;
        private readonly HotelApp.API.Services.ICurrentUserService _currentUserService;

        public ClientsController(Func<IClient, IHotelClientService> clientServiceFactory, HotelApp.API.Services.ICurrentUserService currentUserService)
        {
            _clientServiceFactory = clientServiceFactory;
            _currentUserService = currentUserService;
        }

        [HttpGet("{name}")]
        public ActionResult<ClientInfoDto> GetClient(string name)
        {
            var client = new ApiClient(name, 0);
            IHotelClientService service = _clientServiceFactory(client);
            double balance = service.GetBalance();
            return Ok(new ClientInfoDto(name, balance));
        }

        [HttpGet("{name}/orders")]
        public ActionResult<IEnumerable<RoomDto>> GetOrders(string name)
        {
            var client = new ApiClient(name, 0);
            IHotelClientService service = _clientServiceFactory(client);
            var rooms = service.GetMyOrders()
                .Select(r => new RoomDto(r.Number, r.Price, r.IsOccupied, r.GetType().Name));

            return Ok(rooms);
        }

        [HttpPost("{name}/bookings")]
        public ActionResult<OperationResultDto> BookRoom(string name, [FromBody] BookRoomRequest request)
        {
            var client = new ApiClient(name, 0);
            IHotelClientService service = _clientServiceFactory(client);

            if (!service.TryBookRoom(request.RoomNumber, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Booking failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }

        [HttpPost("{name}/payments")]
        public ActionResult<PayRoomResponse> PayForRoom(string name, [FromBody] PayRoomRequest request)
        {
            var client = new ApiClient(name, 0);
            IHotelClientService service = _clientServiceFactory(client);

            if (!service.TryPayForRoom(request.RoomNumber, request.StayDays, out double paidAmount, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Payment failed"));
            }

            double balance = service.GetBalance();
            return Ok(new PayRoomResponse(paidAmount, balance));
        }

        
    }
}
