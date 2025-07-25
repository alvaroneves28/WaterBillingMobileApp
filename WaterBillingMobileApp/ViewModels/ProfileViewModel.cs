using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
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

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand LoadProfileCommand { get; }
        public IAsyncRelayCommand SaveProfileCommand { get; }
        public IAsyncRelayCommand ChangePasswordCommand { get; }


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
                    await Shell.Current.DisplayAlert("Success", "Password updated successfully.", "OK");
                else
                    await Shell.Current.DisplayAlert("Error", "Failed to update password. Please check current password.", "OK");
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

    }
}
