using System.Security.Claims;

namespace HotelApp.API.Services
{
    public interface ICurrentUserService
    {
        string? GetName();
        string? GetRole();
        ClaimsPrincipal? GetPrincipal();
    }
}
