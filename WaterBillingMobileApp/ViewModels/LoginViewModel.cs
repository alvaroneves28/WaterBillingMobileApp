using System.ComponentModel;
using WaterBillingMobileApp.Model;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new Command(async () => await LoginAsync());
        }

        public string Email { get; set; }
        public string Password { get; set; }

        public Command LoginCommand { get; }

        private async Task LoginAsync()
        {
            try
            {
                var request = new LoginRequest { Email = Email, Password = Password };
                var response = await _authService.LoginAsync(request);

                if (!string.IsNullOrEmpty(response?.Token))
                {
                    // Guardar token no SecureStorage
                    await SecureStorage.SetAsync("auth_token", response.Token);

                    // Navegar para a página principal
                    Application.Current.MainPage = new NavigationPage(new MainPage());

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Erro", "Credenciais inválidas.", "OK");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                await App.Current.MainPage.DisplayAlert("Erro", "Falha ao autenticar: " + ex.Message, "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
