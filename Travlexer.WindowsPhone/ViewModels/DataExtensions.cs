using System.Windows;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class DataExtensions
    {
        public static ManagementSections GetManagementSection(DependencyObject obj)
        {
            return (ManagementSections) obj.GetValue(ManagementSectionProperty);
        }

        public static void SetManagementSection(DependencyObject obj, ManagementSections value)
        {
            obj.SetValue(ManagementSectionProperty, value);
        }

        public static readonly DependencyProperty ManagementSectionProperty = DependencyProperty.RegisterAttached(
            "ManagementSection",
            typeof (ManagementSections),
            typeof (DataExtensions),
            null);
    }
}