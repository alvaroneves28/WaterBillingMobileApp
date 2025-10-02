using System.ComponentModel;
using System.Windows.Input;
using WaterBillingMobileApp.Model;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModel
{
    /// <summary>
    /// ViewModel for the Login page.
    /// Handles user authentication, credential validation, and navigation to other authentication-related pages.
    /// Implements INotifyPropertyChanged for property change notifications.
    /// </summary>
    public class LoginViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Authentication service for handling login operations and token management.
        /// </summary>
        private readonly AuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// Sets up commands for login, password recovery, and meter request navigation.
        /// </summary>
        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new Command(async () => await LoginAsync());
            ForgotPasswordCommand = new Command(async () => await NavigateToForgotPasswordAsync());
            RequestMeterCommand = new Command(async () => await NavigateToAnonymousRequestAsync());
        }

        /// <summary>
        /// Backing field for the Email property.
        /// </summary>
        private string _email;

        /// <summary>
        /// Gets or sets the user's email address for authentication.
        /// </summary>
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

        /// <summary>
        /// Backing field for the Password property.
        /// </summary>
        private string _password;

        /// <summary>
        /// Gets or sets the user's password for authentication.
        /// </summary>
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

        /// <summary>
        /// Backing field for the IsBusy property.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether a login operation is in progress.
        /// Used to disable UI elements during authentication.
        /// </summary>
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

        /// <summary>
        /// Gets the command to perform user login.
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Gets the command to navigate to the forgot password page.
        /// </summary>
        public ICommand ForgotPasswordCommand { get; }

        /// <summary>
        /// Gets the command to navigate to the anonymous meter request page.
        /// </summary>
        public ICommand RequestMeterCommand { get; }

        /// <summary>
        /// Performs user login with validation and error handling.
        /// Validates email format and required fields before making API call.
        /// Stores authentication token securely upon successful login.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Validate email is not empty
                if (string.IsNullOrWhiteSpace(Email))
                {
                    await ShowAlert("Email Required", "Please enter your email address.", "OK");
                    return;
                }

                // Validate password is not empty
                if (string.IsNullOrWhiteSpace(Password))
                {
                    await ShowAlert("Password Required", "Please enter your password.", "OK");
                    return;
                }

                // Validate email format
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

                // Specific error messages based on exception content
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

        /// <summary>
        /// Validates an email address format using MailAddress parsing.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        /// <returns>True if the email format is valid, false otherwise.</returns>
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

        /// <summary>
        /// Navigates to the main application page after successful login.
        /// Ensures navigation occurs on the main UI thread.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Navigates to the anonymous meter request page.
        /// Handles both modal and standard navigation depending on current page structure.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task NavigateToAnonymousRequestAsync()
        {
            try
            {
                // Check current page type
                var currentPage = Application.Current?.MainPage;

                if (currentPage is NavigationPage navPage)
                {
                    // Use standard navigation if already in NavigationPage
                    await navPage.Navigation.PushAsync(new WaterBillingMobileApp.Views.AnonymousRequestPage());
                }
                else if (currentPage != null)
                {
                    // Use modal navigation otherwise
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

        /// <summary>
        /// Navigates to the forgot password page.
        /// Handles both modal and standard navigation depending on current page structure.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Displays an alert dialog with the specified title, message, and button text.
        /// Ensures the alert is shown on the main UI thread.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message content.</param>
        /// <param name="button">The button text.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}