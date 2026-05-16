using HotelApp.API.DTOs;
using HotelApp.Interfaces;
using HotelApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly IHotelAdminService _hotelAdminService;
        private readonly HotelApp.API.Services.ICurrentUserService _currentUserService;

        public AdminsController(IHotelAdminService hotelAdminService, HotelApp.API.Services.ICurrentUserService currentUserService)
        {
            _hotelAdminService = hotelAdminService;
            _currentUserService = currentUserService;
        }

        [HttpGet("revenue")]
        public ActionResult<RevenueResponse> GetRevenue()
        {
            _hotelAdminService.TryGetRevenue(out double revenue, out string? errorMessage);
            if (errorMessage != null)
            {
                return BadRequest(new OperationResultDto(false, errorMessage));
            }

            return Ok(new RevenueResponse(revenue));
        }

        [HttpPost("rooms")]
        public ActionResult<OperationResultDto> AddRoom([FromBody] AddRoomRequest request)
        {
            if (!RoomTypeRegistry.TryGet(request.TypeCode, out RoomTypeDefinition? definition) || definition == null)
            {
                return BadRequest(new OperationResultDto(false, "Unknown room type"));
            }

            Room room = definition.Factory(request.Number, request.Price);
            if (!_hotelAdminService.TryAddRoom(room, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Add room failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }

        [HttpDelete("rooms/{number:int}")]
        public ActionResult<OperationResultDto> RemoveRoom(int number)
        {
            if (!_hotelAdminService.TryRemoveRoom(number, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Remove room failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }

        [HttpPost("{name}/change-password")]
        public ActionResult<OperationResultDto> ChangePassword(string name, [FromBody] ChangePasswordRequest request)
        {
            if (!_hotelAdminService.TryChangePasswordByName(name, request.CurrentPassword, request.NewPassword, out string? errorMessage))
            {
                if (errorMessage == "Помилка: адміністратора не знайдено в БД.")
                {
                    return NotFound(new OperationResultDto(false, "Admin not found"));
                }

                return BadRequest(new OperationResultDto(false, errorMessage ?? "Change password failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }
    }
}
