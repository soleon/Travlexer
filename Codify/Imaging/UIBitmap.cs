using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Codify.Imaging
{
	public class UIBitmap
	{
		#region Public Property

		public static UIElement GetContent(DependencyObject obj)
		{
			return (UIElement) obj.GetValue(ContentProperty);
		}

		public static void SetContent(DependencyObject obj, UIElement value)
		{
			obj.SetValue(ContentProperty, value);
		}

		public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
			"Content",
			typeof (UIElement),
			typeof (UIBitmap),
			new PropertyMetadata(default(UIElement), OnContentChanged));

		private static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
#if DEBUG
            if (DesignerProperties.IsInDesignTool) return;
#endif

		    if (!(sender is ImageBrush))
		        throw new InvalidOperationException("UIBitmap.Content can only be attached to an ImageBrush.");
		    var brush = (ImageBrush) sender;
			var element = e.NewValue as UIElement;
		    if (element == null) return;

		    brush.ImageSource = new WriteableBitmap(element, null);
		}

		#endregion
	}
}
