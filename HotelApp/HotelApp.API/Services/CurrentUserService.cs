using System.Security.Claims;

namespace HotelApp.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? GetPrincipal()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        public string? GetName()
        {
            return GetPrincipal()?.Identity?.Name;
        }

        public string? GetRole()
        {
            return GetPrincipal()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}
