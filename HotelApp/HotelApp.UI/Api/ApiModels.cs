namespace HotelApp.UI.Api
{
    public record AuthRequest(int RoleId, string Name, string Password);
    public record AuthResponse(bool Success, string? Name, string? Error);

    public record RoomDto(int Number, double Price, bool IsOccupied, string Type);
    public record ClientInfoDto(string Name, double Balance);
    public record RevenueResponse(double Revenue);

    public record BookRoomRequest(int RoomNumber);
    public record PayRoomRequest(int RoomNumber, int StayDays);
    public record PayRoomResponse(double PaidAmount, double Balance);

    public record AddRoomRequest(int Number, double Price, string TypeCode);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    public record OperationResultDto(bool Success, string? Error);

    public record ApiResult(bool Success, string? Error);
    public record ApiResult<T>(bool Success, T? Data, string? Error);
}
