using System.Windows;
using System.Windows.Controls;

namespace Travlexer.WindowsPhone.Controls
{
    public class CircleButton : Button
    {
        public CircleButton()
        {
            DefaultStyleKey = typeof (CircleButton);
        }

        public string Caption
        {
            get { return (string) GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption",
            typeof (string),
            typeof (CircleButton),
            null);
    }
}