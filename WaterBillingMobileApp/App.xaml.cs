using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());

            // Inicializar serviço de notificações
            InitializeNotificationService();
        }

        private void InitializeNotificationService()
        {
            try
            {
                // Obter o serviço através do DI
                var authService = Handler.MauiContext?.Services.GetService<AuthService>();
                if (authService != null)
                {
                    var notificationService = new NotificationService(authService);

                    // Verificar novas faturas quando a app inicia
                    _ = notificationService.CheckOnAppStartAsync();

                    // Iniciar verificações periódicas
                    notificationService.StartPeriodicCheck();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar NotificationService: {ex.Message}");
            }
        }
    }
}