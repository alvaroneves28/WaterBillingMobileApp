using System.Net.Http;
using System.Text;
using System.Text.Json;
using WaterBillingMobileApp.Model;

namespace WaterBillingMobileApp.Services
{
    public class AuthService
    {
        private const string BaseUrl = "https://10.0.2.2:44328/api/";

        private readonly HttpClient _httpClient;

        public AuthService()
        {
            var handler = new HttpClientHandler();
            // Ignorar erros do certificado (só para desenvolvimento)
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(BaseUrl + "Auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Se não vier token válido, tratar como falha
                    throw new Exception("Token inválido recebido da API");
                }

                return loginResponse;
            }
            else
            {
                // Para ajudar no debug, mostra o status code e a resposta
                throw new Exception($"Erro no login: {response.StatusCode} - {responseContent}");
            }
        }

    }
}
