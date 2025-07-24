using System.ComponentModel;
using System.Windows.Input;
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

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (_email == value) return;
                _email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password == value) return;
                _password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }

        public ICommand LoginCommand { get; }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Warning", "Please fill in all fields.", "OK");
                return;
            }

            try
            {
                var request = new LoginRequest { Email = Email, Password = Password };
                var response = await _authService.LoginAsync(request);

                if (!string.IsNullOrEmpty(response?.Token))
                {
                    await SecureStorage.SetAsync("auth_token", response.Token);

                    // Replace MainPage with Shell and navigate to dashboard
                    Application.Current.MainPage = new AppShell();
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Login Failed", "Invalid credentials.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Authentication failed: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
