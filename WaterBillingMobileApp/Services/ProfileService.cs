using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

public class ProfileService : IProfileService
{
    private readonly HttpClient _httpClient;

    public ProfileService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProfileDTO> GetProfileAsync()
    {
        var response = await _httpClient.GetAsync("Profile");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProfileDTO>();
    }

    public async Task UpdateProfileAsync(UpdateProfileRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("Profile", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateEmailAsync(UpdateEmailRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("Profile/email", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("Profile/password", request);
        return response.IsSuccessStatusCode;
    }

    public async Task UpdateProfileImageAsync(UpdateProfileImageRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("Profile/image", request);
        response.EnsureSuccessStatusCode();
    }



}
