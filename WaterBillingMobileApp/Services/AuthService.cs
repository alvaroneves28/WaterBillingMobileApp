using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WaterBillingMobileApp.Model;

namespace WaterBillingMobileApp.Services
{
    public class AuthService
    {
        private const string BaseUrl = "https://10.0.2.2:44328/api/";
        private const string TokenKey = "jwt_token";

        private readonly HttpClient _httpClient;

        public AuthService()
        {
            var handler = new HttpClientHandler();
            // Ignorar erros de certificado (apenas para desenvolvimento)
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        /// <summary>
        /// Faz login e guarda automaticamente o token JWT.
        /// </summary>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    throw new Exception("Token inválido recebido da API");
                }

                await SecureStorage.SetAsync(TokenKey, loginResponse.Token); 

                return loginResponse;
            }
            else
            {
                throw new Exception($"Erro no login: {response.StatusCode} - {responseContent}");
            }
        }

        /// <summary>
        /// Recupera o token guardado (ou string vazia).
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(TokenKey) ?? string.Empty;
        }

        /// <summary>
        /// Verifica se existe token guardado.
        /// </summary>
        public async Task<bool> IsLoggedIn()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            return !string.IsNullOrWhiteSpace(token);
        }

        /// <summary>
        /// Apaga o token (logout).
        /// </summary>
        public void Logout()
        {
            SecureStorage.Remove(TokenKey);
        }

        /// <summary>
        /// Cria um HttpClient com o token JWT já configurado.
        /// </summary>
        public static async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            if (string.IsNullOrEmpty(token))
                throw new Exception("Token não encontrado. Utilizador não autenticado.");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
