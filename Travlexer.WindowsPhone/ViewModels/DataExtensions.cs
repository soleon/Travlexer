using System.Windows;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class DataExtensions
    {
        public static ManagementSection GetManagementSection(DependencyObject obj)
        {
            return (ManagementSection) obj.GetValue(ManagementSectionProperty);
        }

        public static void SetManagementSection(DependencyObject obj, ManagementSection value)
        {
            obj.SetValue(ManagementSectionProperty, value);
        }

        public static readonly DependencyProperty ManagementSectionProperty = DependencyProperty.RegisterAttached(
            "ManagementSection",
            typeof (ManagementSection),
            typeof (DataExtensions),
            null);
    }
}