using System;
using System.Globalization;
using System.Windows.Data;

namespace Codify.Converters
{
    /// <summary>
    ///     Converts between local DateTime to UTC DateTime.
    /// </summary>
    public class LocalDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime)) throw new ArgumentException("This converter only supports converting System.DateTime object.", "value");
            return ((DateTime) value).ToUniversalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime)) throw new ArgumentException("This converter only supports converting System.DateTime object.", "value");
            return ((DateTime) value).ToLocalTime();
        }
    }
}