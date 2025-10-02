using System.Globalization;

namespace WaterBillingMobileApp.Converter
{
    /// <summary>
    /// Converter that inverts a boolean value.
    /// Used in XAML bindings to invert boolean properties (e.g., IsEnabled based on IsBusy).
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to its inverse.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">Culture information (not used).</param>
        /// <returns>The inverted boolean value if input is bool, otherwise returns the original value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return value;
        }

        /// <summary>
        /// Converts back an inverted boolean value to its original state.
        /// </summary>
        /// <param name="value">The inverted boolean value.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">Culture information (not used).</param>
        /// <returns>The original boolean value if input is bool, otherwise returns the original value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return value;
        }
    }
}