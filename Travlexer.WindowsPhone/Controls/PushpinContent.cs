using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Controls
{
	/// <summary>
	/// Represents a pushpin on a map.
	/// </summary>
	public class PushpinContent : Control
	{
		


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PushpinContent"/> class.
		/// </summary>
		public PushpinContent()
		{
			DefaultStyleKey = typeof (PushpinContent);
#if DEBUG
			if (!DesignerProperties.IsInDesignTool)
			{
				return;
			}
			var pin = new DesignTime().UserPin;
			Title = pin.Name;
			Address = pin.FormattedAddress;
			Icon = pin.Icon;
			State = PushpinContentStates.Expanded;
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
			typeof (PushpinContent),
			null);

		public object CommandParameterViewDetail
		{
			get { return GetValue(CommandParameterViewDetailProperty); }
			set { SetValue(CommandParameterViewDetailProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterViewDetailProperty = DependencyProperty.Register(
			"CommandParameterViewDetail",
			typeof (object),
			typeof (PushpinContent),
			null);

		public ICommand CommandDepart
		{
			get { return (ICommand) GetValue(CommandDepartProperty); }
			set { SetValue(CommandDepartProperty, value); }
		}

		public static readonly DependencyProperty CommandDepartProperty = DependencyProperty.Register(
			"CommandDepart",
			typeof (ICommand),
			typeof (PushpinContent),
			null);

		public object CommandParameterDepart
		{
			get { return GetValue(CommandParameterDepartProperty); }
			set { SetValue(CommandParameterDepartProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterDepartProperty = DependencyProperty.Register(
			"CommandParameterDepart",
			typeof (object),
			typeof (PushpinContent),
			null);

		public ICommand CommandArrive
		{
			get { return (ICommand) GetValue(CommandArriveProperty); }
			set { SetValue(CommandArriveProperty, value); }
		}

		public static readonly DependencyProperty CommandArriveProperty = DependencyProperty.Register(
			"CommandArrive",
			typeof (ICommand),
			typeof (PushpinContent),
			null);

		public object CommandParameterArrive
		{
			get { return GetValue(CommandParameterArriveProperty); }
			set { SetValue(CommandParameterArriveProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterArriveProperty = DependencyProperty.Register(
			"CommandParameterArrive",
			typeof (object),
			typeof (PushpinContent),
			null);

		public ICommand CommandDelete
		{
			get { return (ICommand) GetValue(CommandDeleteProperty); }
			set { SetValue(CommandDeleteProperty, value); }
		}

		public static readonly DependencyProperty CommandDeleteProperty = DependencyProperty.Register(
			"CommandDelete",
			typeof (ICommand),
			typeof (PushpinContent),
			null);

		public object CommandParameterDelete
		{
			get { return GetValue(CommandParameterDeleteProperty); }
			set { SetValue(CommandParameterDeleteProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterDeleteProperty = DependencyProperty.Register(
			"CommandParameterDelete",
			typeof (object),
			typeof (PushpinContent),
			null);

		public ICommand CommandPinSearchResult
		{
			get { return (ICommand) GetValue(CommandPinSearchResultProperty); }
			set { SetValue(CommandPinSearchResultProperty, value); }
		}

		public static readonly DependencyProperty CommandPinSearchResultProperty = DependencyProperty.Register(
			"CommandPinSearchResult",
			typeof (ICommand),
			typeof (PushpinContent),
			null);

		public object CommandParameterPinDearchResult
		{
			get { return GetValue(CommandParameterPinDearchResultProperty); }
			set { SetValue(CommandParameterPinDearchResultProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterPinDearchResultProperty = DependencyProperty.Register(
			"CommandParameterPinDearchResult",
			typeof (object),
			typeof (PushpinContent),
			null);

		#endregion


		#region Public Properties

		public PushpinContentStates State
		{
			get { return (PushpinContentStates)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}

		public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
			"State",
			typeof(PushpinContentStates),
			typeof (PushpinContent),
			new PropertyMetadata(default(PushpinContentStates), OnStateChanged));

		public string Title
		{
			get { return (string) GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title",
			typeof (string),
			typeof (PushpinContent),
			null);

		public string Address
		{
			get { return (string) GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		}

		public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
			"Address",
			typeof (string),
			typeof (PushpinContent),
			null);

		public PlaceIcon Icon
		{
			get { return (PlaceIcon) GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
			"Icon",
			typeof (PlaceIcon),
			typeof (PushpinContent),
			new PropertyMetadata(default(PlaceIcon)));

		#endregion


		#region Event Handling

		public override void OnApplyTemplate()
		{
			VisualStateManager.GoToState(this, State.ToString(), false);
			base.OnApplyTemplate();
		}

		private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var content = (PushpinContent) d;
			VisualStateManager.GoToState(content, content.State.ToString(), true);
		}

		#endregion
	}
}
