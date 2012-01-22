using System.Windows;
using System.Windows.Controls;

namespace Travlexer.WindowsPhone.Controls.Icons
{
	public abstract class IconBase : Control
	{
		protected IconBase()
		{
			DefaultStyleKey = typeof(IconBase);
		}

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
			"StrokeThickness",
			typeof(double),
			typeof(IconBase),
			null);
	}
}