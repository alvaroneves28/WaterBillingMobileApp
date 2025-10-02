using System.Globalization;

namespace WaterBillingMobileApp.Converter
{
    /// <summary>
    /// Converter that checks if a string contains content (not null, empty, or whitespace).
    /// Used to conditionally show/hide error messages or validation indicators in the UI.
    /// </summary>
    /// <remarks>
    /// This converter is commonly used with the IsVisible property to display
    /// error messages only when an error string has content.
    /// Example: IsVisible="{Binding ErrorMessage, Converter={StaticResource StringToBoolConverter}}"
    /// </remarks>
    public class StringToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string to a boolean indicating whether it has meaningful content.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">Culture information (not used).</param>
        /// <returns>
        /// True if the string is not null, empty, or whitespace; otherwise false.
        /// Returns false if the value is not a string.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }
            return false;
        }

        /// <summary>
        /// ConvertBack is not implemented as boolean to string conversion is not needed in this application.
        /// </summary>
        /// <param name="value">The boolean value.</param>
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