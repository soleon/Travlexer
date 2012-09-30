using System;
using System.Windows;

namespace Travlexer.WindowsPhone.ViewModels
{
    public static class DataExtensions
    {
        #region Attached Properties

        public static ManagementSection GetManagementSection(DependencyObject obj)
        {
            return (ManagementSection)obj.GetValue(ManagementSectionProperty);
        }

        public static void SetManagementSection(DependencyObject obj, ManagementSection value)
        {
            obj.SetValue(ManagementSectionProperty, value);
        }

        public static readonly DependencyProperty ManagementSectionProperty = DependencyProperty.RegisterAttached(
            "ManagementSection",
            typeof(ManagementSection),
            typeof(DataExtensions),
            null);

        #endregion

        #region Extensions Methods

        /// <summary>
        /// Parse distance integer value in meters to the most appropriate string for display purpose.
        /// </summary>
        /// <param name="value">The distance integer value in meters.</param>
        /// <returns>A string that represents the distance integer value in a readable format according to the current app settings.</returns>
        public static string ToDistanceText(this int value)
        {
            if (value < 1000) return value + (value > 1 ? " meters" : " meter");
            return ((double)value / 1000).ToString("f1") + " km";
        }

        /// <summary>
        /// Parse duration integer value in seconds to the most appropriate string for display purpose.
        /// </summary>
        /// <param name="value">The duration integer value in seconds.</param>
        /// <returns>A string that represents the duration integer value in a readable format according to the current app settings.</returns>
        public static string ToDurationText(this int value)
        {
            var timeSpan = TimeSpan.FromSeconds(value);
            var days = timeSpan.Days;
            var hours = timeSpan.Hours;
            var minutes = timeSpan.Minutes;

            if (days > 0) return days + " d " + hours + " hr " + minutes + " min";
            if (hours > 0) return hours + " hr " + minutes + " min";
            if (minutes > 0) return minutes + " min";
            return value + " sec";
        }

        #endregion
    }
}