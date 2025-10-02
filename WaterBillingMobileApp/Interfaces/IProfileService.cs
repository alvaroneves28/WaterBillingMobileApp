using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterBillingMobileApp.DTO;

namespace WaterBillingMobileApp.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDTO> GetProfileAsync();
        Task UpdateProfileAsync(UpdateProfileRequest request);
        Task UpdateEmailAsync(UpdateEmailRequest request);
        Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request);
        Task UpdateProfileImageAsync(UpdateProfileImageRequest request);
    }
}
