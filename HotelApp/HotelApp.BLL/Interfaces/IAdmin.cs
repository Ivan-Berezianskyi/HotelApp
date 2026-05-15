namespace HotelApp.Interfaces
{
    public interface IAdmin : IAccount
    {
        bool ChangePassword(string currentPassword, string newPassword);
    }
}