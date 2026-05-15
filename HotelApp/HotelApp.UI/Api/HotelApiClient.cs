using System.Net.Http.Json;
using HotelApp.UI.Interfaces;

namespace HotelApp.UI.Api
{
    internal class HotelApiClient : IHotelApiClient
    {
        private readonly HttpClient _httpClient;

        public HotelApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResult<AuthResponse>> AuthenticateAsync(int roleId, string name, string password)
        {
            var request = new AuthRequest(roleId, name, password);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/authenticate", request);

            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return new ApiResult<AuthResponse>(true, payload, null);
            }

            var errorPayload = await response.Content.ReadFromJsonAsync<AuthResponse>();
            string? error = errorPayload?.Error ?? "Authentication failed";
            return new ApiResult<AuthResponse>(false, errorPayload, error);
        }

        public async Task<ApiResult<IReadOnlyList<RoomDto>>> GetRoomsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("api/hotels");
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<List<RoomDto>>() ?? new List<RoomDto>();
                return new ApiResult<IReadOnlyList<RoomDto>>(true, payload, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult<IReadOnlyList<RoomDto>>(false, null, error);
        }

        public async Task<ApiResult<ClientInfoDto>> GetClientAsync(string name)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/clients/{Uri.EscapeDataString(name)}");
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<ClientInfoDto>();
                return new ApiResult<ClientInfoDto>(true, payload, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult<ClientInfoDto>(false, null, error);
        }

        public async Task<ApiResult<IReadOnlyList<RoomDto>>> GetClientOrdersAsync(string name)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/clients/{Uri.EscapeDataString(name)}/orders");
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<List<RoomDto>>() ?? new List<RoomDto>();
                return new ApiResult<IReadOnlyList<RoomDto>>(true, payload, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult<IReadOnlyList<RoomDto>>(false, null, error);
        }

        public async Task<ApiResult> BookRoomAsync(string name, int roomNumber)
        {
            var request = new BookRoomRequest(roomNumber);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/clients/{Uri.EscapeDataString(name)}/bookings", request);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult(true, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult(false, error);
        }

        public async Task<ApiResult<PayRoomResponse>> PayForRoomAsync(string name, int roomNumber, int stayDays)
        {
            var request = new PayRoomRequest(roomNumber, stayDays);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/clients/{Uri.EscapeDataString(name)}/payments", request);
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<PayRoomResponse>();
                return new ApiResult<PayRoomResponse>(true, payload, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult<PayRoomResponse>(false, null, error);
        }

        public async Task<ApiResult> AddRoomAsync(AddRoomRequest request)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/admins/rooms", request);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult(true, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult(false, error);
        }

        public async Task<ApiResult> RemoveRoomAsync(int roomNumber)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/admins/rooms/{roomNumber}");
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult(true, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult(false, error);
        }

        public async Task<ApiResult<RevenueResponse>> GetRevenueAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("api/admins/revenue");
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<RevenueResponse>();
                return new ApiResult<RevenueResponse>(true, payload, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult<RevenueResponse>(false, null, error);
        }

        public async Task<ApiResult> ChangePasswordAsync(string name, ChangePasswordRequest request)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/admins/{Uri.EscapeDataString(name)}/change-password", request);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult(true, null);
            }

            string? error = await TryReadErrorAsync(response);
            return new ApiResult(false, error);
        }

        private static async Task<string?> TryReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var op = await response.Content.ReadFromJsonAsync<OperationResultDto>();
                if (op != null && !string.IsNullOrWhiteSpace(op.Error))
                {
                    return op.Error;
                }
            }
            catch
            {
            }

            string content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(content) ? "Request failed" : content;
        }
    }
}
