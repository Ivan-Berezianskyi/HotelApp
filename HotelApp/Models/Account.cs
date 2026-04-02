using HotelApp.Interfaces;

namespace HotelApp.Models
{
    internal abstract class Account : IAccount
    {
        public string Name { get; protected set; }
        protected string _password;
        public Account(string name, string password)
        {
            Name = name;
            _password = password;
        }

        public bool CheckPassword(string pass) => _password == pass;
    }
}
