using HotelApp.API.DTOs;
using HotelApp.API.Models;
using HotelApp.Data;
using HotelApp.Data.Entities;
using HotelApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly HotelDbContext _dbContext;
        private readonly Func<IClient, IHotelClientService> _clientServiceFactory;

        public ClientsController(HotelDbContext dbContext, Func<IClient, IHotelClientService> clientServiceFactory)
        {
            _dbContext = dbContext;
            _clientServiceFactory = clientServiceFactory;
        }

        [HttpGet("{name}")]
        public ActionResult<ClientInfoDto> GetClient(string name)
        {
            DbUser? user = GetClientUser(name);
            if (user == null)
            {
                return NotFound(new OperationResultDto(false, "Client not found"));
            }

            return Ok(new ClientInfoDto(user.Name, user.Balance ?? 0));
        }

        [HttpGet("{name}/orders")]
        public ActionResult<IEnumerable<RoomDto>> GetOrders(string name)
        {
            DbUser? user = GetClientUser(name);
            if (user == null)
            {
                return NotFound(new OperationResultDto(false, "Client not found"));
            }

            var client = new ApiClient(user.Name, user.Balance ?? 0);
            IHotelClientService service = _clientServiceFactory(client);
            var rooms = service.GetMyOrders()
                .Select(r => new RoomDto(r.Number, r.Price, r.IsOccupied, r.GetType().Name));

            return Ok(rooms);
        }

        [HttpPost("{name}/bookings")]
        public ActionResult<OperationResultDto> BookRoom(string name, [FromBody] BookRoomRequest request)
        {
            DbUser? user = GetClientUser(name);
            if (user == null)
            {
                return NotFound(new OperationResultDto(false, "Client not found"));
            }

            var client = new ApiClient(user.Name, user.Balance ?? 0);
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
            DbUser? user = GetClientUser(name);
            if (user == null)
            {
                return NotFound(new OperationResultDto(false, "Client not found"));
            }

            var client = new ApiClient(user.Name, user.Balance ?? 0);
            IHotelClientService service = _clientServiceFactory(client);

            if (!service.TryPayForRoom(request.RoomNumber, request.StayDays, out double paidAmount, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Payment failed"));
            }

            return Ok(new PayRoomResponse(paidAmount, client.Money));
        }

        private DbUser? GetClientUser(string name)
        {
            string normalizedName = name.Trim().ToLowerInvariant();
            return _dbContext.Users.AsNoTracking().FirstOrDefault(user =>
                user.Role.ToLower() == "client"
                && user.Name.ToLower() == normalizedName);
        }
    }
}
