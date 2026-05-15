using HotelApp.UI.Api;

namespace HotelApp.UI.Interfaces
{
    public interface IHotelApiClient
    {
        Task<ApiResult<AuthResponse>> AuthenticateAsync(int roleId, string name, string password);
        Task<ApiResult<IReadOnlyList<RoomDto>>> GetRoomsAsync();
        Task<ApiResult<ClientInfoDto>> GetClientAsync(string name);
        Task<ApiResult<IReadOnlyList<RoomDto>>> GetClientOrdersAsync(string name);
        Task<ApiResult> BookRoomAsync(string name, int roomNumber);
        Task<ApiResult<PayRoomResponse>> PayForRoomAsync(string name, int roomNumber, int stayDays);
        Task<ApiResult> AddRoomAsync(AddRoomRequest request);
        Task<ApiResult> RemoveRoomAsync(int roomNumber);
        Task<ApiResult<RevenueResponse>> GetRevenueAsync();
        Task<ApiResult> ChangePasswordAsync(string name, ChangePasswordRequest request);
    }
}
