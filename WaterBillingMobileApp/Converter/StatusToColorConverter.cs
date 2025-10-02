using System.Globalization;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.Converter
{
    /// <summary>
    /// Converter that maps MeterStatus enum values to corresponding colors.
    /// Used to display status badges with appropriate colors in the UI.
    /// </summary>
    /// <remarks>
    /// Supports both MeterStatus enum and string representations.
    /// Color mapping:
    /// - Pending: Orange
    /// - Approved: Green
    /// - Rejected: Red
    /// - Unknown: Gray
    /// </remarks>
    public class StatusToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a MeterStatus enum value or string to its corresponding color.
        /// </summary>
        /// <param name="value">The MeterStatus value or string representation to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">Culture information (not used).</param>
        /// <returns>
        /// A Color object representing the status:
        /// <list type="bullet">
        /// <item><description>Orange for Pending status</description></item>
        /// <item><description>Green for Approved status</description></item>
        /// <item><description>Red for Rejected status</description></item>
        /// <item><description>Gray for unknown or invalid values</description></item>
        /// </list>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Handle MeterStatus enum directly
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

            // Handle string representation of MeterStatus
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

            // Default color for null or invalid values
            return Colors.Gray;
        }

        /// <summary>
        /// ConvertBack is not implemented as color to status conversion is not needed in this application.
        /// </summary>
        /// <param name="value">The color value.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <exception cref="NotImplementedException">This method is not supported for this converter.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}