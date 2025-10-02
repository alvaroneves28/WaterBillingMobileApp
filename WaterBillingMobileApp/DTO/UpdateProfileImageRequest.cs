namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating a user's profile image.
    /// Used in ProfilePage when the user uploads a new profile photo.
    /// </summary>
    public class UpdateProfileImageRequest
    {
        /// <summary>
        /// Gets or sets the profile image URL or base64 encoded image string.
        /// Can be a remote URL, local file path, or a data URI with base64 encoded image data.
        /// Format example: "data:image/jpeg;base64,/9j/4AAQSkZJRg..."
        /// </summary>
        public string ProfileImageUrl { get; set; }
    }
}