using HotelApp.Interfaces;
using HotelApp.Security;

namespace HotelApp.Models
{
    internal class Admin : Account, IAdmin
    {
        public Admin(string name, string password) : base(name, password) { }

        public bool ChangePassword(string currentPassword, string newPassword)
        {
            if (!CheckPassword(currentPassword))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 4)
            {
                return false;
            }

            _password = PasswordHasher.Hash(newPassword);
            return true;
        }
    }
}
