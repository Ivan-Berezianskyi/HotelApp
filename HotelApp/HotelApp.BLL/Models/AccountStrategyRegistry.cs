using HotelApp.Interfaces;

namespace HotelApp.Models
{
    using HotelApp.Services;
    public static class AccountStrategyRegistry
    {
        private static readonly IAccountStrategy[] Strategies =
        [
            new AdminAccountStrategy(),
            new ClientAccountStrategy()
        ];
        public static IAccountStrategy? GetStrategy(IAccount account)
        {
            return Strategies.FirstOrDefault(s => s.IsApplicable(account));
        }
        public static IAccountStrategy? GetStrategyByRoleId(int roleId)
        {
            return Strategies.FirstOrDefault(s => s.GetRoleId() == roleId);
        }
        public static IReadOnlyList<IAccountStrategy> GetAllStrategies()
        {
            return Array.AsReadOnly(Strategies);
        }
    }
}

namespace HotelApp.Services { }
