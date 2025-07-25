using Microsoft.Extensions.Logging;
using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.Views;

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
            builder.Services.AddSingleton<ProfilePage>();
            builder.Services.AddSingleton<AuthService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
