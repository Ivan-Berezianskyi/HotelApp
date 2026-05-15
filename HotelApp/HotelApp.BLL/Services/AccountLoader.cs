using HotelApp.Data;
using HotelApp.Data.Entities;
using HotelApp.Interfaces;
using HotelApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Services
{
    public class AccountLoader : IAccountLoader
    {
        private readonly HotelDbContext _dbContext;

        public AccountLoader(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<IAccount> LoadAccountsFromDb()
        {
            List<IAccount> accounts = new List<IAccount>();

            List<DbUser> users = _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Bookings)
                .ToList();
            
            foreach (DbUser user in users)
            {
                if (string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    accounts.Add(new Admin(user.Name, user.PasswordHash));
                    continue;
                }

                if (string.Equals(user.Role, "client", StringComparison.OrdinalIgnoreCase))
                {
                    accounts.Add(new Client(user.Name, user.PasswordHash, user.Balance ?? 0));
                }
            }

            return accounts;
        }
    }
}
