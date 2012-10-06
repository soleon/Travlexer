using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.Converters
{
    public class ElementColorKeyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ElementColor)) return null;
            return ApplicationContext.Data.ElementColorMap.First(i => i.Key.Equals(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is KeyValuePair<ElementColor, string>)) return null;
            return ((KeyValuePair<ElementColor, string>)value).Key;
        }
    }
}