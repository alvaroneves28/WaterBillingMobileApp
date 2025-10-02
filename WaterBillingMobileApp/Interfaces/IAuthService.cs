using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterBillingMobileApp.Model;

namespace WaterBillingMobileApp.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<string> GetTokenAsync();
        Task<bool> IsLoggedIn();
        void Logout();
        Task<HttpClient> CreateAuthenticatedClientAsync();
    }
}
