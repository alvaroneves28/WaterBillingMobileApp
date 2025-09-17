using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private ProfileService _profileService;
        private readonly AuthService _authService;
        private readonly INavigation _navigation;

        public ProfileViewModel(AuthService authService, INavigation navigation)
        {
            _authService = authService;
            _navigation = navigation;
            LoadProfileCommand = new AsyncRelayCommand(LoadProfileAsync);
            SaveProfileCommand = new AsyncRelayCommand(SaveProfileAsync);
            ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync);
            ChangeEmailCommand = new AsyncRelayCommand(ChangeEmailAsync);

            _ = InitializeAsync(); // Carrega tudo no arranque
        }

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private string profileImagePath;

        [ObservableProperty]
        private string address;

        [ObservableProperty]
        private string currentPassword;

        [ObservableProperty]
        private string newPassword;

        // Novas propriedades adicionadas
        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string newEmail;

        [ObservableProperty]
        private string currentPasswordForEmail;

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand LoadProfileCommand { get; }
        public IAsyncRelayCommand SaveProfileCommand { get; }
        public IAsyncRelayCommand ChangePasswordCommand { get; }
        public IAsyncRelayCommand ChangeEmailCommand { get; } // Novo comando

        private async Task InitializeAsync()
        {
            try
            {
                var httpClient = await _authService.CreateAuthenticatedClientAsync();
                _profileService = new ProfileService(httpClient);
                await LoadProfileAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
            }
        }

        private async Task LoadProfileAsync()
        {
            if (IsBusy || _profileService == null) return;

            try
            {
                IsBusy = true;
                var profile = await _profileService.GetProfileAsync();

                FullName = profile.FullName;
                Email = profile.Email;
                Address = profile.Address;
                PhoneNumber = profile.Phone;
                ProfileImagePath = profile.ProfileImagePath;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error loading profile: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveProfileAsync()
        {
            if (IsBusy || _profileService == null) return;

            try
            {
                IsBusy = true;

                var updateRequest = new UpdateProfileRequest
                {
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    Address = Address
                };

                await _profileService.UpdateProfileAsync(updateRequest);
                await Shell.Current.DisplayAlert("Success", "Profile updated successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error saving profile: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ChangePasswordAsync()
        {
            if (IsBusy || _profileService == null) return;

            // Validações adicionadas
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your current password.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
            {
                await Shell.Current.DisplayAlert("Error", "New password must be at least 6 characters long.", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "New password and confirmation don't match.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var passwordRequest = new UpdatePasswordRequest
                {
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword
                };

                var success = await _profileService.UpdatePasswordAsync(passwordRequest);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Password updated successfully.", "OK");

                    // Limpar campos após sucesso
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to update password. Please check your current password.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error changing password: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Nova funcionalidade: Alterar Email
        private async Task ChangeEmailAsync()
        {
            if (IsBusy || _profileService == null) return;

            // Validações
            if (string.IsNullOrWhiteSpace(NewEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a new email address.", "OK");
                return;
            }

            if (!IsValidEmail(NewEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentPasswordForEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your current password for verification.", "OK");
                return;
            }

            if (NewEmail.Equals(Email, StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.DisplayAlert("Error", "The new email is the same as your current email.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var emailRequest = new UpdateEmailRequest
                {
                    NewEmail = NewEmail,
                    CurrentPassword = CurrentPasswordForEmail
                };

                await _profileService.UpdateEmailAsync(emailRequest);

                await Shell.Current.DisplayAlert("Success", "Email updated successfully.", "OK");

                // Atualizar o email exibido e limpar campos
                Email = NewEmail;
                NewEmail = string.Empty;
                CurrentPasswordForEmail = string.Empty;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error changing email: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Método auxiliar para validar email
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
    }
}



