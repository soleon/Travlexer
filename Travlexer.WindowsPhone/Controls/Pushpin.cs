using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Travlexer.WindowsPhone.Infrustructure.Entities;

namespace Travlexer.WindowsPhone.Controls
{
	/// <summary>
	/// Represents a pushpin on a map.
	/// </summary>
	public class Pushpin : Control
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Pushpin"/> class.
		/// </summary>
		public Pushpin()
		{
			DefaultStyleKey = typeof (Pushpin);
#if DEBUG
			if (!DesignerProperties.IsInDesignTool)
			{
				return;
			}
			var pin = new DesignTime().UserPin;
			Title = pin.Name;
			Address = pin.Address.FormattedAddress;
			Icon = pin.Icon;
			Color = pin.Color;
#endif
		}

		#endregion


		#region Public Properties

		public string Title
		{
			get { return (string) GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title",
			typeof (string),
			typeof (Pushpin),
			null);

		public string Address
		{
			get { return (string) GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		}

		public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
			"Address",
			typeof (string),
			typeof (Pushpin),
			null);

		public PlaceIcon Icon
		{
			get { return (PlaceIcon) GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
			"Icon",
			typeof (PlaceIcon),
			typeof (Pushpin),
			new PropertyMetadata(default(PlaceIcon)));

		public PlaceColor Color
		{
			get { return (PlaceColor) GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
			"Color",
			typeof (PlaceColor),
			typeof (Pushpin),
			null);

		#endregion
	}
}
