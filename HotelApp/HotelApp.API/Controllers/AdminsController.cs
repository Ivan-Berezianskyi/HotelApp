using HotelApp.API.DTOs;
using HotelApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly IHotelFacade _hotelFacade;
        private readonly Services.ICurrentUserService _currentUserService;

        public AdminsController(IHotelFacade hotelFacade, Services.ICurrentUserService currentUserService)
        {
            _hotelFacade = hotelFacade;
            _currentUserService = currentUserService;
        }

        [HttpGet("revenue")]
        public ActionResult<RevenueResponse> GetRevenue()
        {
            _hotelFacade.TryGetRevenue(out double revenue, out string? errorMessage);
            if (errorMessage != null)
            {
                return BadRequest(new OperationResultDto(false, errorMessage));
            }

            return Ok(new RevenueResponse(revenue));
        }

        [HttpPost("rooms")]
        public ActionResult<OperationResultDto> AddRoom([FromBody] AddRoomRequest request)
        {
            if (!_hotelFacade.TryAddRoom(request.TypeCode, request.Number, request.Price, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Add room failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }

        [HttpDelete("rooms/{number:int}")]
        public ActionResult<OperationResultDto> RemoveRoom(int number)
        {
            if (!_hotelFacade.TryRemoveRoom(number, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Remove room failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }

        [HttpPost("{name}/change-password")]
        public ActionResult<OperationResultDto> ChangePassword(string name, [FromBody] ChangePasswordRequest request)
        {
            if (!_hotelFacade.TryChangePasswordByName(name, request.CurrentPassword, request.NewPassword, out string? errorMessage))
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
