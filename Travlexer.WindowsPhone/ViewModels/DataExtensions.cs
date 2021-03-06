using System;
using System.Globalization;
using System.Windows;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public static class DataExtensions
    {
        #region Attached Properties

        public static ManagementSection GetManagementSection(DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException("obj", "Unable to get management section on null dependency object");
            return (ManagementSection) obj.GetValue(ManagementSectionProperty);
        }

        public static void SetManagementSection(DependencyObject obj, ManagementSection value)
        {
            if (obj == null) throw new ArgumentNullException("obj", "Unable to set management section on null dependency object");
            obj.SetValue(ManagementSectionProperty, value);
        }

        public static readonly DependencyProperty ManagementSectionProperty = DependencyProperty.RegisterAttached(
            "ManagementSection",
            typeof (ManagementSection),
            typeof (DataExtensions),
            null);

        #endregion


        #region Extensions Methods

        /// <summary>
        ///     Parse distance integer value in meters to the most appropriate string for display purpose.
        /// </summary>
        /// <param name="value"> The distance integer value in meters. </param>
        /// <returns> A string that represents the distance integer value in a readable format according to the current app settings. </returns>
        public static string ToDistanceText(this int value)
        {
            switch (ApplicationContext.Data.UnitSystem.Value)
            {
                case UnitSystems.Metric:
                    if (value < 1000) return value + (value > 1 ? " meters" : " meter");
                    return ((double) value/1000).ToString("f1", CultureInfo.CurrentCulture) + " km";
                case UnitSystems.Imperial:

                    var yards = value*0.9144D;
                    var miles = yards/1760D;
                    if (miles < 0.5) return yards.ToString("f1", CultureInfo.CurrentCulture) + (yards > 1 ? " yards" : " yard");
                    return miles.ToString("f1", CultureInfo.CurrentCulture) + " mi";
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Parse duration integer value in seconds to the most appropriate string for display purpose.
        /// </summary>
        /// <param name="value"> The duration integer value in seconds. </param>
        /// <returns> A string that represents the duration integer value in a readable format according to the current app settings. </returns>
        public static string ToDurationText(this int value)
        {
            var duration = TimeSpan.FromSeconds(value);
            var days = duration.Days;
            var hours = duration.Hours;
            var minutes = duration.Minutes;

            if (days > 0) return days + " d " + hours + " hr " + minutes + " min";
            if (hours > 0) return hours + " hr " + minutes + " min";
            if (minutes > 0) return minutes + " min";
            return value + " sec";
        }

        #endregion
    }
}