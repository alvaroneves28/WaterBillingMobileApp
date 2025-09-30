using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class ResetPasswordPage : ContentPage
{
    public ResetPasswordPage()
    {
        InitializeComponent();

        // Resolver o serviço via DI
        var service = (ResetPasswordService)App.Current.Handler.MauiContext.Services
                        .GetService(typeof(ResetPasswordService))!;

        // Associar ViewModel com serviço
        BindingContext = new ResetPasswordViewModel(service);
    }
}

