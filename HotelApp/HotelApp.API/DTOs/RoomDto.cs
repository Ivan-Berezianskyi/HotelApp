namespace HotelApp.API.DTOs
{
    public record RoomDto(int Number, double Price, bool IsOccupied, string Type);
}
