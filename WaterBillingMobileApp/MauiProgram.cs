using Microsoft.Extensions.Logging;
using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.Views;
using WaterBillingMobileApp.ViewModels;
using WaterBillingMobileApp.ViewModel;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services com interfaces
            builder.Services.AddSingleton<ResetPasswordService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

            // ProfileService precisa de HttpClient, então registre assim:
            builder.Services.AddTransient<IProfileService>(sp =>
            {
                var authService = sp.GetRequiredService<IAuthService>();
                var httpClient = authService.CreateAuthenticatedClientAsync().GetAwaiter().GetResult();
                return new ProfileService(httpClient);
            });

            // ViewModels
            builder.Services.AddTransient<SubmitReadingViewModel>();
            builder.Services.AddTransient<AnonymousRequestViewModel>();
            builder.Services.AddTransient<ConsumptionHistoryViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<InvoiceDetailViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<RatesAndStatusViewModel>();
            builder.Services.AddTransient<ResetPasswordViewModel>();
            builder.Services.AddTransient<AboutViewModel>();


            // Pages
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AnonymousRequestPage>();
            builder.Services.AddTransient<ConsumptionHistoryPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<InvoiceDetailPage>();
            builder.Services.AddTransient<InvoicesPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MeterReadingPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<RatesAndStatusPage>();
            builder.Services.AddTransient<ResetPasswordPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}