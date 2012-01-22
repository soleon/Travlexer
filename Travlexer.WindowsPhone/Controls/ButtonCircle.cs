using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Shell;

namespace Travlexer.WindowsPhone.Controls
{
	public class ButtonCircle : Button
	{
		public ButtonCircle()
		{
			DefaultStyleKey = typeof (ButtonCircle);
		}

		public string Caption
		{
			get { return (string) GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
			"Caption",
			typeof (string),
			typeof (ButtonCircle),
			null);
	}
}
