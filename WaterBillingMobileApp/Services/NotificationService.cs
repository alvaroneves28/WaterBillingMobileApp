using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.Services
{
    public class NotificationService
    {
        private readonly AuthService _authService;
        private DateTime _lastCheckTime;
        private const string LAST_CHECK_KEY = "last_invoice_check";
        private const string LAST_INVOICE_COUNT_KEY = "last_invoice_count";

        public NotificationService(AuthService authService)
        {
            _authService = authService;
            LoadLastCheckTime();
        }

        /// <summary>
        /// Verifica se há novas faturas desde a última verificação
        /// </summary>
        public async Task<bool> CheckForNewInvoicesAsync()
        {
            try
            {
                // Verificar se o utilizador está autenticado
                if (!await _authService.IsLoggedIn())
                    return false;

                var httpClient = await _authService.CreateAuthenticatedClientAsync();
                var invoices = await httpClient.GetFromJsonAsync<List<InvoiceDTO>>("Customer/invoices");

                if (invoices == null || invoices.Count == 0)
                    return false;

                // Verificar se há faturas mais recentes que a última verificação
                var newInvoices = invoices.Where(i => i.IssueDate > _lastCheckTime).ToList();

                // Verificar também se há mais faturas que antes (método alternativo)
                var lastInvoiceCount = await GetLastInvoiceCountAsync();
                var hasNewInvoicesByCount = invoices.Count > lastInvoiceCount;

                if (newInvoices.Any() || hasNewInvoicesByCount)
                {
                    // Atualizar a data da última verificação
                    _lastCheckTime = DateTime.Now;
                    await SaveLastCheckTimeAsync();
                    await SaveInvoiceCountAsync(invoices.Count);

                    // Mostrar notificação para as novas faturas
                    await ShowNewInvoiceNotificationAsync(newInvoices.Count > 0 ? newInvoices.Count : 1);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log do erro (em produção, usar um logger adequado)
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar novas faturas: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Inicia verificação automática de novas faturas (a cada 30 minutos)
        /// </summary>
        public void StartPeriodicCheck()
        {
            var timer = new Timer(async _ => await CheckForNewInvoicesAsync(),
                                 null,
                                 TimeSpan.Zero, // Inicia imediatamente
                                 TimeSpan.FromMinutes(30)); // Repete a cada 30 minutos
        }

        /// <summary>
        /// Verifica novas faturas quando a app é aberta
        /// </summary>
        public async Task CheckOnAppStartAsync()
        {
            // Aguardar um pouco para a app carregar completamente
            await Task.Delay(3000);
            await CheckForNewInvoicesAsync();
        }

        /// <summary>
        /// Mostra uma notificação local para novas faturas
        /// </summary>
        private async Task ShowNewInvoiceNotificationAsync(int invoiceCount)
        {
            string message = invoiceCount == 1
                ? "You have a new invoice available!"
                : $"You have {invoiceCount} new invoices available!";

            // Usar o sistema de alertas do MAUI
            await Shell.Current.DisplayAlert(
                "New Invoice",
                message + " Tap to view your invoices.",
                "View Now",
                "Later");

            // Se o utilizador escolher "View Now", navegar para a página de faturas
            // Esta parte pode ser implementada com mais sofisticação usando eventos
        }

        /// <summary>
        /// Carrega a última data de verificação do storage local
        /// </summary>
        private void LoadLastCheckTime()
        {
            var lastCheckString = Preferences.Get(LAST_CHECK_KEY, string.Empty);

            if (DateTime.TryParse(lastCheckString, out var lastCheck))
            {
                _lastCheckTime = lastCheck;
            }
            else
            {
                // Se não há registro anterior, usar uma data antiga para pegar todas as faturas
                _lastCheckTime = DateTime.Now.AddDays(-30);
            }
        }

        /// <summary>
        /// Guarda a data da última verificação
        /// </summary>
        private async Task SaveLastCheckTimeAsync()
        {
            Preferences.Set(LAST_CHECK_KEY, _lastCheckTime.ToString("O"));
            await Task.CompletedTask;
        }

        /// <summary>
        /// Obtém o número de faturas da última verificação
        /// </summary>
        private async Task<int> GetLastInvoiceCountAsync()
        {
            return Preferences.Get(LAST_INVOICE_COUNT_KEY, 0);
        }

        /// <summary>
        /// Guarda o número atual de faturas
        /// </summary>
        private async Task SaveInvoiceCountAsync(int count)
        {
            Preferences.Set(LAST_INVOICE_COUNT_KEY, count);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Limpa os dados de notificações (útil no logout)
        /// </summary>
        public void ClearNotificationData()
        {
            Preferences.Remove(LAST_CHECK_KEY);
            Preferences.Remove(LAST_INVOICE_COUNT_KEY);
            _lastCheckTime = DateTime.Now.AddDays(-30);
        }

        /// <summary>
        /// Força uma verificação manual (útil para testing)
        /// </summary>
        public async Task ForceCheckAsync()
        {
            _lastCheckTime = DateTime.Now.AddDays(-1); // Força a verificação
            await CheckForNewInvoicesAsync();
        }
    }
}