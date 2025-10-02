using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Profile page.
    /// Manages user profile information including personal details, email, password, and profile photo.
    /// Provides functionality to view and update all profile-related data.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm.
    /// </summary>
    public partial class ProfileViewModel : ObservableObject
    {
        /// <summary>
        /// Service for profile-related API operations.
        /// </summary>
        private ProfileService _profileService;

        /// <summary>
        /// Authentication service for creating authenticated HTTP clients.
        /// </summary>
        private readonly IAuthService _authService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileViewModel"/> class.
        /// Sets up all commands and initiates asynchronous profile loading.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="navigation">The navigation service.</param>
        public ProfileViewModel(IAuthService authService)
        {
            _authService = authService;
            LoadProfileCommand = new AsyncRelayCommand(LoadProfileAsync);
            SaveProfileCommand = new AsyncRelayCommand(SaveProfileAsync);
            ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync);
            ChangeEmailCommand = new AsyncRelayCommand(ChangeEmailAsync);
            ChangePhotoCommand = new AsyncRelayCommand(ChangePhotoAsync);

            _ = InitializeAsync();
        }

        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        [ObservableProperty]
        private string fullName;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [ObservableProperty]
        private string email;

        /// <summary>
        /// Gets or sets the user's phone number.
        /// </summary>
        [ObservableProperty]
        private string phoneNumber;

        /// <summary>
        /// Gets or sets the file path or URL of the user's profile image.
        /// </summary>
        [ObservableProperty]
        private string profileImagePath;

        /// <summary>
        /// Gets or sets the user's address.
        /// </summary>
        [ObservableProperty]
        private string address;

        /// <summary>
        /// Gets or sets the current password for verification purposes.
        /// </summary>
        [ObservableProperty]
        private string currentPassword;

        /// <summary>
        /// Gets or sets the new password when changing password.
        /// </summary>
        [ObservableProperty]
        private string newPassword;

        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// Must match NewPassword for the change to be accepted.
        /// </summary>
        [ObservableProperty]
        private string confirmPassword;

        /// <summary>
        /// Gets or sets the new email address when changing email.
        /// </summary>
        [ObservableProperty]
        private string newEmail;

        /// <summary>
        /// Gets or sets the current password for email change verification.
        /// </summary>
        [ObservableProperty]
        private string currentPasswordForEmail;

        /// <summary>
        /// Gets or sets a value indicating whether an operation is in progress.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets the command to load profile data from the API.
        /// </summary>
        public IAsyncRelayCommand LoadProfileCommand { get; }

        /// <summary>
        /// Gets the command to save updated profile information.
        /// </summary>
        public IAsyncRelayCommand SaveProfileCommand { get; }

        /// <summary>
        /// Gets the command to change the user's password.
        /// </summary>
        public IAsyncRelayCommand ChangePasswordCommand { get; }

        /// <summary>
        /// Gets the command to change the user's email address.
        /// </summary>
        public IAsyncRelayCommand ChangeEmailCommand { get; }

        /// <summary>
        /// Gets the command to change the user's profile photo.
        /// </summary>
        public IAsyncRelayCommand ChangePhotoCommand { get; }

        /// <summary>
        /// Initializes the profile service with an authenticated HTTP client and loads profile data.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Allows the user to select and upload a new profile photo.
        /// Converts the selected image to base64 format and updates it via the API.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ChangePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a profile photo"
                });

                if (result != null)
                {
                    // Read image stream
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);

                    // Convert to Base64
                    var imageBytes = memoryStream.ToArray();
                    var base64Image = Convert.ToBase64String(imageBytes);
                    var imageUrl = $"data:image/jpeg;base64,{base64Image}";

                    // Update via API
                    var request = new UpdateProfileImageRequest
                    {
                        ProfileImageUrl = imageUrl
                    };

                    await _profileService.UpdateProfileImageAsync(request);

                    // Update UI
                    ProfileImagePath = imageUrl;

                    await Shell.Current.DisplayAlert("Success", "Profile photo updated successfully.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update photo: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Loads the user's profile information from the API.
        /// Updates all profile properties with the retrieved data.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Saves updated profile information (name, phone number, address) to the API.
        /// Does not include email or password changes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Changes the user's password after validating current password and new password requirements.
        /// Requires password confirmation and minimum length validation.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ChangePasswordAsync()
        {
            if (IsBusy || _profileService == null) return;

            // Validate current password
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your current password.", "OK");
                return;
            }

            // Validate new password length
            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
            {
                await Shell.Current.DisplayAlert("Error", "New password must be at least 6 characters long.", "OK");
                return;
            }

            // Validate password confirmation
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

                    // Clear fields after success
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

        /// <summary>
        /// Changes the user's email address after validation and password verification.
        /// Requires current password for security purposes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ChangeEmailAsync()
        {
            if (IsBusy || _profileService == null) return;

            // Validate new email is not empty
            if (string.IsNullOrWhiteSpace(NewEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a new email address.", "OK");
                return;
            }

            // Validate email format
            if (!IsValidEmail(NewEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            // Validate password for verification
            if (string.IsNullOrWhiteSpace(CurrentPasswordForEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your current password for verification.", "OK");
                return;
            }

            // Check if new email is different from current
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

                // Update displayed email and clear fields
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
    }
}