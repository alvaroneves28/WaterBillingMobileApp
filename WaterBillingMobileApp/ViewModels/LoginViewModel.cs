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
            ForgotPasswordCommand = new Command(async () => await NavigateToForgotPasswordAsync());
            RequestMeterCommand = new Command(async () => await NavigateToAnonymousRequestAsync());
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

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand RequestMeterCommand { get; }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Validações
                if (string.IsNullOrWhiteSpace(Email))
                {
                    await ShowAlert("Email Required", "Please enter your email address.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    await ShowAlert("Password Required", "Please enter your password.", "OK");
                    return;
                }

                // Validar formato do email
                if (!IsValidEmail(Email))
                {
                    await ShowAlert("Invalid Email", "Please enter a valid email address.", "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Attempting login for: {Email}");

                var request = new LoginRequest { Email = Email, Password = Password };
                var response = await _authService.LoginAsync(request);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    System.Diagnostics.Debug.WriteLine("Login successful, token received");

                    await SecureStorage.SetAsync("auth_token", response.Token);

                    // Clear password field for security
                    Password = string.Empty;

                    // Navigate to main page
                    await NavigateToMainPage();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Login failed: No token received");
                    await ShowAlert("Login Failed", "Invalid response from server. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Mensagens de erro específicas
                string errorMessage = "Unable to login. Please try again.";
                string errorTitle = "Login Error";

                if (ex.Message.Contains("Invalid credentials") ||
                    ex.Message.Contains("Unauthorized") ||
                    ex.Message.Contains("401"))
                {
                    errorTitle = "Invalid Credentials";
                    errorMessage = "The email or password you entered is incorrect. Please check and try again.";
                }
                else if (ex.Message.Contains("Account is inactive"))
                {
                    errorTitle = "Account Inactive";
                    errorMessage = "Your account is currently inactive. Please contact support.";
                }
                else if (ex.Message.Contains("conexão") ||
                         ex.Message.Contains("connection") ||
                         ex.Message.Contains("Timeout"))
                {
                    errorTitle = "Connection Error";
                    errorMessage = "Unable to connect to the server. Please check your internet connection and try again.";
                }
                else if (ex.Message.Contains("API está a correr"))
                {
                    errorTitle = "Server Unavailable";
                    errorMessage = "The server is currently unavailable. Please try again later.";
                }

                await ShowAlert(errorTitle, errorMessage, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task NavigateToMainPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Navigating to main page");

                // Check if running on main thread
                if (MainThread.IsMainThread)
                {
                    Application.Current.MainPage = new AppShell();
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        Application.Current.MainPage = new AppShell();
                        await Shell.Current.GoToAsync("//MainPage");
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                throw;
            }
        }

        private async Task NavigateToAnonymousRequestAsync()
        {
            try
            {
                // Verificar qual é a página atual
                var currentPage = Application.Current?.MainPage;

                if (currentPage is NavigationPage navPage)
                {
                    // Se já estiver em NavigationPage, usar a navegação normal
                    await navPage.Navigation.PushAsync(new WaterBillingMobileApp.Views.AnonymousRequestPage());
                }
                else if (currentPage != null)
                {
                    // Se não estiver em NavigationPage, usar PushModalAsync
                    await currentPage.Navigation.PushModalAsync(
                        new NavigationPage(new WaterBillingMobileApp.Views.AnonymousRequestPage())
                    );
                }
                else
                {
                    await ShowAlert("Error", "Navigation not available", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await ShowAlert("Error", $"Navigation error: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToForgotPasswordAsync()
        {
            try
            {
                var currentPage = Application.Current?.MainPage;

                if (currentPage is NavigationPage navPage)
                {
                    await navPage.Navigation.PushAsync(new WaterBillingMobileApp.Views.ForgotPasswordPage());
                }
                else if (currentPage != null)
                {
                    await currentPage.Navigation.PushModalAsync(
                        new NavigationPage(new WaterBillingMobileApp.Views.ForgotPasswordPage())
                    );
                }
                else
                {
                    await ShowAlert("Error", "Navigation not available", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await ShowAlert("Error", $"Navigation error: {ex.Message}", "OK");
            }
        }

        private async Task ShowAlert(string title, string message, string button)
        {
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    if (MainThread.IsMainThread)
                    {
                        await Application.Current.MainPage.DisplayAlert(title, message, button);
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Application.Current.MainPage.DisplayAlert(title, message, button);
                        });
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Alert: {title} - {message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowAlert error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}