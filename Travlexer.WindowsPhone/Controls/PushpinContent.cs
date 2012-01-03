using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Controls
{
	/// <summary>
	/// Represents a pushpin on a map.
	/// </summary>
	[TemplateVisualState(GroupName = GROUP_VisualStates, Name = STATE_Expanded)]
	[TemplateVisualState(GroupName = GROUP_VisualStates, Name = STATE_Collapsed)]
	[TemplateVisualState(GroupName = GROUP_PinTypeStates, Name = STATE_SearchResult)]
	[TemplateVisualState(GroupName = GROUP_PinTypeStates, Name = STATE_UserPin)]
	[TemplateVisualState(GroupName = GROUP_WorkingStates, Name = STATE_Idle)]
	[TemplateVisualState(GroupName = GROUP_WorkingStates, Name = STATE_Working)]
	[TemplateVisualState(GroupName = GROUP_WorkingStates, Name = STATE_Error)]
	[TemplatePart(Name = PART_Content, Type = typeof (Panel))]
	[TemplatePart(Name = PART_SearchResultIndicator, Type = typeof (Control))]
	[TemplatePart(Name = PART_Title, Type = typeof (TextBlock))]
	[TemplatePart(Name = PART_Address, Type = typeof (TextBlock))]
	[TemplatePart(Name = PART_GeoCoordinate, Type = typeof (TextBlock))]
	[TemplatePart(Name = PART_WorkingText, Type = typeof (TextBlock))]
	[TemplatePart(Name = PART_ButtonPanel, Type = typeof (Panel))]
	[TemplatePart(Name = PART_ButtonDelete, Type = typeof (Button))]
	[TemplatePart(Name = PART_ButtonPin, Type = typeof (Button))]
	public class PushpinContent : Control
	{
		#region MyRegion

		// Visual states and groups name.
		private const string GROUP_VisualStates = "VisualStates";
		private const string STATE_Expanded = "Expanded";
		private const string STATE_Collapsed = "Collapsed";
		private const string GROUP_PinTypeStates = "PinTypeStates";
		private const string STATE_SearchResult = "SearchResult";
		private const string STATE_UserPin = "UserPin";
		private const string GROUP_WorkingStates = "WorkingStates";
		private const string STATE_Idle = "Idle";
		private const string STATE_Working = "Working";
		private const string STATE_Error = "Error";

		// Template parts name.
		private const string PART_Content = "Content";
		private const string PART_SearchResultIndicator = "SearchResultIndicator";
		private const string PART_Title = "Title";
		private const string PART_Address = "Address";
		private const string PART_GeoCoordinate = "GeoCoordinate";
		private const string PART_WorkingText = "WorkingText";
		private const string PART_ButtonPanel = "ButtonPanel";
		private const string PART_ButtonDelete = "ButtonDelete";
		private const string PART_ButtonPin = "ButtonPin";

		#endregion


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
			VisualState = PushpinOverlayVisualStates.Expanded;
#endif
		}

		#endregion


		#region Commands

		public ICommand CommandViewDetails
		{
			get { return (ICommand) GetValue(CommandViewDetailsProperty); }
			set { SetValue(CommandViewDetailsProperty, value); }
		}

		public static readonly DependencyProperty CommandViewDetailsProperty = DependencyProperty.Register(
			"CommandViewDetails",
			typeof (ICommand),
			typeof (PushpinContent),
			null);

		public object CommandViewDetailsParameter
		{
			get { return GetValue(CommandViewDetailsParameterProperty); }
			set { SetValue(CommandViewDetailsParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandViewDetailsParameterProperty = DependencyProperty.Register(
			"CommandViewDetailsParameter",
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

		public object CommandDepartParameter
		{
			get { return GetValue(CommandDepartParameterProperty); }
			set { SetValue(CommandDepartParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandDepartParameterProperty = DependencyProperty.Register(
			"CommandDepartParameter",
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

		public object CommandArriveParameter
		{
			get { return GetValue(CommandArriveParameterProperty); }
			set { SetValue(CommandArriveParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandArriveParameterProperty = DependencyProperty.Register(
			"CommandArriveParameter",
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

		public object CommandDeleteParameter
		{
			get { return GetValue(CommandDeleteParameterProperty); }
			set { SetValue(CommandDeleteParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandDeleteParameterProperty = DependencyProperty.Register(
			"CommandDeleteParameter",
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

		public object CommandPinSearchResultParameter
		{
			get { return GetValue(CommandPinSearchResultParameterProperty); }
			set { SetValue(CommandPinSearchResultParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandPinSearchResultParameterProperty = DependencyProperty.Register(
			"CommandPinSearchResultParameter",
			typeof (object),
			typeof (PushpinContent),
			null);

		#endregion


		#region Public Properties

		public PushpinOverlayVisualStates VisualState
		{
			get { return (PushpinOverlayVisualStates) GetValue(VisualStateProperty); }
			set { SetValue(VisualStateProperty, value); }
		}

		public static readonly DependencyProperty VisualStateProperty = DependencyProperty.Register(
			"VisualState",
			typeof (PushpinOverlayVisualStates),
			typeof (PushpinContent),
			new PropertyMetadata(default(PushpinOverlayVisualStates), OnVisualStateChanged));

		public PushpinOverlayWorkingStates WorkingState
		{
			get { return (PushpinOverlayWorkingStates) GetValue(WorkingStateProperty); }
			set { SetValue(WorkingStateProperty, value); }
		}

		public static readonly DependencyProperty WorkingStateProperty = DependencyProperty.Register(
			"WorkingState",
			typeof (PushpinOverlayWorkingStates),
			typeof (PushpinContent),
			new PropertyMetadata(default(PushpinOverlayWorkingStates), OnWorkingStateChanged));

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

		public bool IsSearchResult
		{
			get { return (bool) GetValue(IsSearchResultProperty); }
			set { SetValue(IsSearchResultProperty, value); }
		}

		public static readonly DependencyProperty IsSearchResultProperty = DependencyProperty.Register(
			"IsSearchResult",
			typeof (bool),
			typeof (PushpinContent),
			new PropertyMetadata(false, OnIsSearchResultChanged));

		#endregion


		#region Event Handling

		public override void OnApplyTemplate()
		{
			VisualStateManager.GoToState(this, VisualState.ToString(), false);
			VisualStateManager.GoToState(this, WorkingState.ToString(), false);
			VisualStateManager.GoToState(this, IsSearchResult ? STATE_SearchResult : STATE_UserPin, false);
			base.OnApplyTemplate();
		}

		private static void OnVisualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var content = (PushpinContent) d;
			VisualStateManager.GoToState(content, content.VisualState.ToString(), true);
		}

		private static void OnWorkingStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var content = (PushpinContent) sender;
			VisualStateManager.GoToState(content, content.WorkingState.ToString(), true);
		}

		private static void OnIsSearchResultChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var content = (PushpinContent) sender;
			VisualStateManager.GoToState(content, content.IsSearchResult ? STATE_SearchResult : STATE_UserPin, true);
		}

		#endregion
	}
}
