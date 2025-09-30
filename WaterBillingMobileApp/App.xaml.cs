using System.Text;
using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp
{

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // MainPage deve ser sempre AppShell
            MainPage = new AppShell();
        }

        public void HandleDeepLink(string url)
        {
            try
            {
                var uri = new Uri(url);
                if (uri.Host == "reset-password")
                {
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    var token = query["token"];
                    var email = query["email"];

                    // Guardar token e email no serviço
                    var service = (ResetPasswordService)App.Current.Handler.MauiContext.Services.GetService(typeof(ResetPasswordService));
                    service.Token = token;
                    service.Email = email;

                    // Navegar apenas para a página sem parâmetros
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (Shell.Current != null)
                            await Shell.Current.GoToAsync($"/{nameof(ResetPasswordPage)}");
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling deep link: {ex.Message}");
            }
        }

    }
}