using System.Globalization;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.Converter
{
    /// <summary>
    /// Converter 3: Converte MeterStatus em cor
    /// Usado para colorir o badge de status
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MeterStatus status)
            {
                return status switch
                {
                    MeterStatus.Pending => Colors.Orange,
                    MeterStatus.Approved => Colors.Green,
                    MeterStatus.Rejected => Colors.Red,
                    _ => Colors.Gray
                };
            }

            // Se for string, tentar converter
            if (value is string statusString)
            {
                if (Enum.TryParse<MeterStatus>(statusString, true, out var parsedStatus))
                {
                    return parsedStatus switch
                    {
                        MeterStatus.Pending => Colors.Orange,
                        MeterStatus.Approved => Colors.Green,
                        MeterStatus.Rejected => Colors.Red,
                        _ => Colors.Gray
                    };
                }
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
