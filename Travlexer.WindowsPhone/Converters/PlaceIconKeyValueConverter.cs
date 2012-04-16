using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.Converters
{
    public class PlaceIconKeyValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is PlaceIcon))
            {
                return null;
            }
            return ApplicationContext.Data.PlaceIconMap.First(i => i.Key.Equals(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is KeyValuePair<PlaceIcon, string>))
            {
                return null;
            }
            return ((KeyValuePair<PlaceIcon, string>) value).Key;
        }

        #endregion
    }
}