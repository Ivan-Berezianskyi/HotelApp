using HotelApp.Models;
using HotelApp.Interfaces;

namespace HotelApp.UI
{
    internal class LoginMenu : BaseMenu, ILoginMenu
    {
        private readonly IAuthService _authService;
        public IAccount? AuthenticatedAccount { get; private set; }
        public bool UserWantsToExit { get; private set; }

        public LoginMenu(IAuthService authService, ILogger logger) : base(logger)
        {
            _authService = authService;
        }

        public override void Display()
        {
            UserWantsToExit = false;
            
            while (AuthenticatedAccount == null && !UserWantsToExit)
            {
                _logger.Print("\n=== СИСТЕМА ГОТЕЛЮ ===");
                _logger.Print("1. Увійти як Адмін | 2. Увійти як Клієнт | 0. Вимкнути систему");
                _logger.Print("Ваш вибір: ");
                
                TryReadNonNegativeNumber(out int action);

                if (action == 0)
                {
                    UserWantsToExit = true;
                    break;
                }

                if (action == 1 || action == 2)
                {
                    string name = GetName();
                    string password = GetPassword();
                    IAccount? account = _authService.Authenticate(action, name, password);

                    if (account != null)
                    {
                        _logger.Print("Авторизація успішна!");
                        AuthenticatedAccount = account;
                        break;
                    }
                    else
                    {
                        ShowError();
                    }
                }
                else
                {
                    _logger.Print("Невідома дія. Спробуйте ще раз.");
                }
            }
        }

        private string GetName()
        {
            _logger.Print("Введіть ім'я: ");
            return Console.ReadLine() ?? string.Empty;
        }

        private string GetPassword()
        {
            _logger.Print("Введіть пароль: ");
            return Console.ReadLine() ?? string.Empty;
        }

        private void ShowError()
        {
            _logger.Print("Доступ відхилено! Невірні ім'я, пароль або вибір.");
        }

        public void ResetState()
        {
            AuthenticatedAccount = null;
            UserWantsToExit = false;
        }
    }
}
