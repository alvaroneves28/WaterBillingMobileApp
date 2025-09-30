using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class ResetPasswordPage : ContentPage
{
    public ResetPasswordPage()
    {
        InitializeComponent();

        // Resolver o servi�o via DI
        var service = (ResetPasswordService)App.Current.Handler.MauiContext.Services
                        .GetService(typeof(ResetPasswordService))!;

        // Associar ViewModel com servi�o
        BindingContext = new ResetPasswordViewModel(service);
    }
}

