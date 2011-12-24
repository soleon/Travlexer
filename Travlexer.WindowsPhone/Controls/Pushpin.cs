using System.ComponentModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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


		#region Commands

		public ICommand CommandViewDetail
		{
			get { return (ICommand) GetValue(CommandViewDetailProperty); }
			set { SetValue(CommandViewDetailProperty, value); }
		}

		public static readonly DependencyProperty CommandViewDetailProperty = DependencyProperty.Register(
			"CommandViewDetail",
			typeof (ICommand),
			typeof (Pushpin),
			null);

		public object CommandParameterViewDetail
		{
			get { return GetValue(CommandParameterViewDetailProperty); }
			set { SetValue(CommandParameterViewDetailProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterViewDetailProperty = DependencyProperty.Register(
			"CommandParameterViewDetail",
			typeof (object),
			typeof (Pushpin),
			null);

		public ICommand CommandDepart
		{
			get { return (ICommand) GetValue(CommandDepartProperty); }
			set { SetValue(CommandDepartProperty, value); }
		}

		public static readonly DependencyProperty CommandDepartProperty = DependencyProperty.Register(
			"CommandDepart",
			typeof (ICommand),
			typeof (Pushpin),
			null);

		public object CommandParameterDepart
		{
			get { return GetValue(CommandParameterDepartProperty); }
			set { SetValue(CommandParameterDepartProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterDepartProperty = DependencyProperty.Register(
			"CommandParameterDepart",
			typeof (object),
			typeof (Pushpin),
			null);

		public ICommand CommandArrive
		{
			get { return (ICommand) GetValue(CommandArriveProperty); }
			set { SetValue(CommandArriveProperty, value); }
		}

		public static readonly DependencyProperty CommandArriveProperty = DependencyProperty.Register(
			"CommandArrive",
			typeof (ICommand),
			typeof (Pushpin),
			null);

		public object CommandParameterArrive
		{
			get { return GetValue(CommandParameterArriveProperty); }
			set { SetValue(CommandParameterArriveProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterArriveProperty = DependencyProperty.Register(
			"CommandParameterArrive",
			typeof (object),
			typeof (Pushpin),
			null);

		public ICommand CommandDelete
		{
			get { return (ICommand) GetValue(CommandDeleteProperty); }
			set { SetValue(CommandDeleteProperty, value); }
		}

		public static readonly DependencyProperty CommandDeleteProperty = DependencyProperty.Register(
			"CommandDelete",
			typeof (ICommand),
			typeof (Pushpin),
			null);

		public object CommandParameterDelete
		{
			get { return GetValue(CommandParameterDeleteProperty); }
			set { SetValue(CommandParameterDeleteProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterDeleteProperty = DependencyProperty.Register(
			"CommandParameterDelete",
			typeof (object),
			typeof (Pushpin),
			null);

		public ICommand CommandPinSearchResult
		{
			get { return (ICommand) GetValue(CommandPinSearchResultProperty); }
			set { SetValue(CommandPinSearchResultProperty, value); }
		}

		public static readonly DependencyProperty CommandPinSearchResultProperty = DependencyProperty.Register(
			"CommandPinSearchResult",
			typeof (ICommand),
			typeof (Pushpin),
			null);

		public object CommandParameterPinDearchResult
		{
			get { return GetValue(CommandParameterPinDearchResultProperty); }
			set { SetValue(CommandParameterPinDearchResultProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterPinDearchResultProperty = DependencyProperty.Register(
			"CommandParameterPinDearchResult",
			typeof (object),
			typeof (Pushpin),
			null);

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

		public GeoCoordinate Location
		{
			get { return (GeoCoordinate) GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}

		public static readonly DependencyProperty LocationProperty = DependencyProperty.Register(
			"Location",
			typeof (GeoCoordinate),
			typeof (Pushpin),
			null);

		#endregion
	}
}
