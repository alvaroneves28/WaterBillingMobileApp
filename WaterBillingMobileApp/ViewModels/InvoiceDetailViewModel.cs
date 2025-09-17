using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class InvoiceDetailViewModel : ObservableObject
    {
        public InvoiceDetailViewModel(InvoiceDTO invoice)
        {
            Invoice = invoice;

            DownloadPdfCommand = new AsyncRelayCommand(DownloadPdfAsync);
            ShareInvoiceCommand = new AsyncRelayCommand(ShareInvoiceAsync);
            BackToInvoicesCommand = new AsyncRelayCommand(BackToInvoicesAsync);

            LoadInvoiceDetails();
        }

        [ObservableProperty]
        private InvoiceDTO invoice;

        [ObservableProperty]
        private InvoiceDetailDTO invoiceDetail;

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand DownloadPdfCommand { get; }
        public IAsyncRelayCommand ShareInvoiceCommand { get; }
        public IAsyncRelayCommand BackToInvoicesCommand { get; }

        public Color StatusColor => Invoice?.Status switch
        {
            InvoiceStatus.Pending => Colors.Orange,
            InvoiceStatus.Approved => Colors.Green,
            InvoiceStatus.Rejected => Colors.Red,
            _ => Colors.Gray
        };

        private void LoadInvoiceDetails()
        {
            // Simular dados detalhados da fatura
            // Em produção, estes dados viriam da API
            InvoiceDetail = new InvoiceDetailDTO
            {
                MeterId = 12345,
                PreviousReading = 1250.5,
                CurrentReading = 1275.3,
                ConsumptionVolume = 24.8,
                PeriodStart = DateTime.Now.AddDays(-30),
                PeriodEnd = DateTime.Now,
                WaterSubtotal = (decimal)Invoice.TotalAmount * 0.7m, 
                ServiceFee = 5.50m,
                TaxAmount = (decimal)Invoice.TotalAmount * 0.23m,    
                TariffBreakdown = new ObservableCollection<TariffBreakdownDTO>
        {
            new TariffBreakdownDTO
            {
                Description = "First 10 m³",
                Volume = 10.0,
                Rate = 0.85m,
                Amount = 8.50m
            },
            new TariffBreakdownDTO
            {
                Description = "Next 14.8 m³",
                Volume = 14.8,
                Rate = 1.20m,
                Amount = 17.76m
            }
        }
            };
        }

        private async Task DownloadPdfAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Simular download de PDF
                await Task.Delay(2000);

                await Shell.Current.DisplayAlert(
                    "Success",
                    $"Invoice #{Invoice.Id} has been downloaded to your device.",
                    "OK");

                // Em produção, implementaria o download real do PDF
                // usando a API e salvando no dispositivo
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to download PDF: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShareInvoiceAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Criar texto para partilhar
                var shareText = $"Invoice #{Invoice.Id}\n" +
                              $"Date: {Invoice.IssueDate:dd/MM/yyyy}\n" +
                              $"Amount: {Invoice.TotalAmount:C}\n" +
                              $"Status: {Invoice.Status}\n" +
                              $"Consumption: {InvoiceDetail.ConsumptionVolume:F1} m³";

                // Usar o sistema de partilha do MAUI
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = $"Invoice #{Invoice.Id}"
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to share invoice: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task BackToInvoicesAsync()
        {
            try
            {
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
            }
        }
    }
}