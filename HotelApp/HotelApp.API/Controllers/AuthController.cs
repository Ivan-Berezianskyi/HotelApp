using HotelApp.API.DTOs;
using HotelApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthResponse> Authenticate([FromBody] AuthRequest req)
        {
            var account = _authService.Authenticate(req.RoleId, req.Name, req.Password);
            if (account == null)
            {
                return Unauthorized(new AuthResponse(false, null, "Invalid credentials"));
            }

            return Ok(new AuthResponse(true, account.Name, null));
        }
    }
}
