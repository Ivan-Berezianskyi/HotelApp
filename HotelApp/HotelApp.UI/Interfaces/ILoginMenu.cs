using HotelApp.UI;

namespace HotelApp.Interfaces
{
    internal interface ILoginMenu : IMenu
    {
        UserSession? AuthenticatedUser { get; }
        bool UserWantsToExit { get; }
        void ResetState();
    }
}