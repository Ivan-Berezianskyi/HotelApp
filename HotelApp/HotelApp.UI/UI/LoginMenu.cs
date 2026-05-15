using HotelApp.Interfaces;
using HotelApp.UI.Interfaces;

namespace HotelApp.UI
{
    internal class LoginMenu : BaseMenu, ILoginMenu
    {
        private readonly IHotelApiClient _apiClient;
        public UserSession? AuthenticatedUser { get; private set; }
        public bool UserWantsToExit { get; private set; }

        public LoginMenu(IHotelApiClient apiClient, ILogger logger) : base(logger)
        {
            _apiClient = apiClient;
        }

        public override void Display()
        {
            UserWantsToExit = false;
            
            while (AuthenticatedUser == null && !UserWantsToExit)
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

                    var authResult = _apiClient.AuthenticateAsync(action, name, password).GetAwaiter().GetResult();
                    if (!authResult.Success || authResult.Data == null)
                    {
                        ShowError(authResult.Error);
                        continue;
                    }

                    double? balance = null;
                    if (action == 2)
                    {
                        var clientResult = _apiClient.GetClientAsync(name).GetAwaiter().GetResult();
                        if (clientResult.Success && clientResult.Data != null)
                        {
                            balance = clientResult.Data.Balance;
                        }
                    }

                    _logger.Print("Авторизація успішна!");
                    AuthenticatedUser = new UserSession(action, name, balance);
                    break;
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

        private void ShowError(string? error = null)
        {
            _logger.Print(error ?? "Доступ відхилено! Невірні ім'я, пароль або вибір.");
        }

        public void ResetState()
        {
            AuthenticatedUser = null;
            UserWantsToExit = false;
        }
    }
}
