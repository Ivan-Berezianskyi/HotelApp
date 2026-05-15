using HotelApp.Interfaces;
using HotelApp.Security;

namespace HotelApp.Models
{
    public abstract class Account : IAccount
    {
        public string Name { get; protected set; }
        protected string _password;
        public Account(string name, string password)
        {
            Name = name;
            _password = password;
        }

        public bool CheckPassword(string pass) => PasswordHasher.Verify(pass, _password);
    }
}
