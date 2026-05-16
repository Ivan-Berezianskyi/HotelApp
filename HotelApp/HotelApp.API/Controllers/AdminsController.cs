using HotelApp.API.DTOs;
using HotelApp.Data;
using HotelApp.Data.Entities;
using HotelApp.Interfaces;
using HotelApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly HotelDbContext _dbContext;
        private readonly IHotelAdminService _hotelAdminService;

        public AdminsController(HotelDbContext dbContext, IHotelAdminService hotelAdminService)
        {
            _dbContext = dbContext;
            _hotelAdminService = hotelAdminService;
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
            string normalizedName = name.Trim().ToLowerInvariant();
            DbUser? user = _dbContext.Users.AsNoTracking().FirstOrDefault(u =>
                u.Role.ToLower() == "admin" && u.Name.ToLower() == normalizedName);
            if (user == null)
            {
                return NotFound(new OperationResultDto(false, "Admin not found"));
            }

            IAdmin admin = new Admin(user.Name, user.PasswordHash);
            if (!_hotelAdminService.TryChangePassword(admin, request.CurrentPassword, request.NewPassword, out string? errorMessage))
            {
                return BadRequest(new OperationResultDto(false, errorMessage ?? "Change password failed"));
            }

            return Ok(new OperationResultDto(true, null));
        }
    }
}
