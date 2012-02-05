using System.Windows;
using System.Windows.Controls.Primitives;

namespace Travlexer.WindowsPhone.Controls
{
	public class CircleToggleButton : ToggleButton
	{
		public CircleToggleButton()
		{
			DefaultStyleKey = typeof(CircleToggleButton);
		}

		public string Caption
		{
			get { return (string) GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
			"Caption",
			typeof (string),
			typeof(CircleToggleButton),
			null);
	}
}