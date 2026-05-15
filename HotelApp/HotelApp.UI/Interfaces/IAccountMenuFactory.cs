using HotelApp.UI;

namespace HotelApp.Interfaces
{
    internal interface IAccountMenuFactory
    {
        IMenu CreateMenu(UserSession session);
    }
}