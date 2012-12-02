using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.Converters
{
    public class ElementColorToBrushConverter : IValueConverter
    {
        public static readonly Dictionary<ElementColor, SolidColorBrush> Colors = new Dictionary<ElementColor, SolidColorBrush>
        {
            {ElementColor.Blue, new SolidColorBrush(Color.FromArgb(0xff, 0x1b, 0xa1, 0xe2))},
            {ElementColor.Magenta, new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x00, 0x97))},
            {ElementColor.Purple, new SolidColorBrush(Color.FromArgb(0xff, 0xa2, 0x00, 0xff))},
            {ElementColor.Teal, new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xab, 0xa9))},
            {ElementColor.Lime, new SolidColorBrush(Color.FromArgb(0xff, 0xbc, 0xbf, 0x26))},
            {ElementColor.Brown, new SolidColorBrush(Color.FromArgb(0xff, 0xa0, 0x50, 0x00))},
            {ElementColor.Pink, new SolidColorBrush(Color.FromArgb(0xff, 0xe6, 0x71, 0xb8))},
            {ElementColor.Orange, new SolidColorBrush(Color.FromArgb(0xff, 0xf0, 0x96, 0x09))},
            {ElementColor.Red, new SolidColorBrush(Color.FromArgb(0xff, 0xe5, 0x14, 0x00))},
            {ElementColor.Green, new SolidColorBrush(Color.FromArgb(0xff, 0x33, 0x99, 0x33))}
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ElementColor))
            {
                return null;
            }
            return Colors[(ElementColor) value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}