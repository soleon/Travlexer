using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codify.WindowsPhone
{
	public class DesignTime
	{
		#region Attached Properties

		public static Brush GetBackground(DependencyObject obj)
		{
			return (Brush) obj.GetValue(BackgroundProperty);
		}

		public static void SetBackground(DependencyObject obj, Brush value)
		{
			obj.SetValue(BackgroundProperty, value);
		}

		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
			"Background",
			typeof (Brush),
			typeof (DesignTime),
#if DEBUG
			new PropertyMetadata(default(Brush), OnBackgroundChanged)
#else
			null
#endif
			);

#if DEBUG
		private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.IsInDesignTool)
			{
				return;
			}
			if (sender is Panel)
			{
				((Panel) sender).Background = (Brush) e.NewValue;
			}
			else if (sender is Control)
			{
				((Control) sender).Background = (Brush) e.NewValue;
			}
		}
#endif

		#endregion
	}
}
