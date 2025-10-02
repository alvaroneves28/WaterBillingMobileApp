using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Invoice Detail page.
    /// Displays comprehensive invoice information including consumption details, tariff breakdown, and payment summary.
    /// Provides functionality to download PDF invoices, share invoice details, and navigate back to the invoice list.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm.
    /// </summary>
    public partial class InvoiceDetailViewModel : ObservableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetailViewModel"/> class.
        /// Sets up commands and loads detailed invoice data.
        /// </summary>
        /// <param name="invoice">The invoice summary to display details for.</param>
        public InvoiceDetailViewModel(InvoiceDTO invoice)
        {
            Invoice = invoice;

            DownloadPdfCommand = new AsyncRelayCommand(DownloadPdfAsync);
            ShareInvoiceCommand = new AsyncRelayCommand(ShareInvoiceAsync);
            BackToInvoicesCommand = new AsyncRelayCommand(BackToInvoicesAsync);

            LoadInvoiceDetails();
        }

        /// <summary>
        /// Gets or sets the invoice summary data.
        /// </summary>
        [ObservableProperty]
        private InvoiceDTO invoice;

        /// <summary>
        /// Gets or sets the detailed invoice information including consumption, tariffs, and charges.
        /// </summary>
        [ObservableProperty]
        private InvoiceDetailDTO invoiceDetail;

        /// <summary>
        /// Gets or sets a value indicating whether an operation is in progress.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets the command to download the invoice as a PDF file.
        /// </summary>
        public IAsyncRelayCommand DownloadPdfCommand { get; }

        /// <summary>
        /// Gets the command to share invoice details via the platform's share functionality.
        /// </summary>
        public IAsyncRelayCommand ShareInvoiceCommand { get; }

        /// <summary>
        /// Gets the command to navigate back to the invoices list page.
        /// </summary>
        public IAsyncRelayCommand BackToInvoicesCommand { get; }

        /// <summary>
        /// Gets the color representation of the invoice status for UI display.
        /// Returns Orange for Pending, Green for Approved, Red for Rejected, and Gray for unknown statuses.
        /// </summary>
        public Color StatusColor => Invoice?.Status switch
        {
            InvoiceStatus.Pending => Colors.Orange,
            InvoiceStatus.Approved => Colors.Green,
            InvoiceStatus.Rejected => Colors.Red,
            _ => Colors.Gray
        };

        /// <summary>
        /// Loads detailed invoice information.
        /// Currently simulates data for demonstration purposes.
        /// In production, this data would be retrieved from the API.
        /// </summary>
        private void LoadInvoiceDetails()
        {
            // Simulate detailed invoice data
            // In production, this data would come from the API
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

        /// <summary>
        /// Downloads the invoice as a PDF file to the device.
        /// Currently simulates the download process.
        /// In production, this would retrieve the PDF from the API and save it to local storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task DownloadPdfAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Simulate PDF download
                await Task.Delay(2000);

                await Shell.Current.DisplayAlert(
                    "Success",
                    $"Invoice #{Invoice.Id} has been downloaded to your device.",
                    "OK");

                // In production, implement actual PDF download
                // using the API and saving to device storage
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

        /// <summary>
        /// Shares invoice details using the platform's native share functionality.
        /// Creates a formatted text summary of the invoice information.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ShareInvoiceAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Create text to share
                var shareText = $"Invoice #{Invoice.Id}\n" +
                              $"Date: {Invoice.IssueDate:dd/MM/yyyy}\n" +
                              $"Amount: {Invoice.TotalAmount:C}\n" +
                              $"Status: {Invoice.Status}\n" +
                              $"Consumption: {InvoiceDetail.ConsumptionVolume:F1} m³";

                // Use MAUI's share system
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

        /// <summary>
        /// Navigates back to the invoices list page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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