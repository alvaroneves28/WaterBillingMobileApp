using Microsoft.Extensions.Logging;
using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.Views;
using WaterBillingMobileApp.ViewModels;
using WaterBillingMobileApp.ViewModel;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp
{
    /// <summary>
    /// Main program class for configuring and building the MAUI application.
    /// Handles dependency injection registration for services, ViewModels, and pages.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the MAUI application.
        /// Registers all services, ViewModels, and pages in the dependency injection container.
        /// </summary>
        /// <returns>A configured <see cref="MauiApp"/> instance ready to run.</returns>
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

            // Register services with their interfaces
            builder.Services.AddSingleton<ResetPasswordService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

            // ProfileService requires HttpClient, so register with factory pattern
            builder.Services.AddTransient<IProfileService>(sp =>
            {
                var authService = sp.GetRequiredService<IAuthService>();
                var httpClient = authService.CreateAuthenticatedClientAsync().GetAwaiter().GetResult();
                return new ProfileService(httpClient);
            });

            // Register ViewModels as transient (new instance per request)
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

            // Register Pages as transient (new instance per navigation)
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
            // Enable debug logging in development builds
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}