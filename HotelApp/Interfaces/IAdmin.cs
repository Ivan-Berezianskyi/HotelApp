namespace HotelApp.Interfaces
{
    internal interface IAdmin : IAccount
    {
        bool ChangePassword(string currentPassword, string newPassword);
    }
}