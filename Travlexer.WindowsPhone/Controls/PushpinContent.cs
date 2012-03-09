using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codify;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.Controls
{
	/// <summary>
	/// Represents a pushpin on a map.
	/// </summary>
	[TemplateVisualState(GroupName = GROUP_VisualStates, Name = STATE_Expanded)]
	[TemplateVisualState(GroupName = GROUP_VisualStates, Name = STATE_Collapsed)]
	[TemplateVisualState(GroupName = GROUP_LoadStates, Name = STATE_Loaded)]
	[TemplateVisualState(GroupName = GROUP_LoadStates, Name = STATE_Loading)]
	[TemplateVisualState(GroupName = GROUP_LoadStates, Name = STATE_Error)]
	[TemplatePart(Name = PART_Content, Type = typeof(Panel))]
	[TemplatePart(Name = PART_SearchResultIndicator, Type = typeof(Control))]
	[TemplatePart(Name = PART_Title, Type = typeof(TextBlock))]
	[TemplatePart(Name = PART_Address, Type = typeof(TextBlock))]
	[TemplatePart(Name = PART_GeoCoordinate, Type = typeof(TextBlock))]
	[TemplatePart(Name = PART_WorkingText, Type = typeof(TextBlock))]
	public class PushpinContent : Control
	{
		#region MyRegion

		// Visual states and groups name.
		private const string GROUP_VisualStates = "VisualStates";
		private const string STATE_Expanded = "Expanded";
		private const string STATE_Collapsed = "Collapsed";
		private const string GROUP_LoadStates = "LoadStates";
		private const string STATE_Loading = "Loading";
		private const string STATE_Error = "Error";
		private const string STATE_Loaded = "Loaded";

		// Template parts name.
		private const string PART_Content = "Content";
		private const string PART_SearchResultIndicator = "SearchResultIndicator";
		private const string PART_Title = "Title";
		private const string PART_Address = "Address";
		private const string PART_GeoCoordinate = "GeoCoordinate";
		private const string PART_WorkingText = "WorkingText";

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PushpinContent"/> class.
		/// </summary>
		public PushpinContent()
		{
			DefaultStyleKey = typeof(PushpinContent);
#if DEBUG
			if (!DesignerProperties.IsInDesignTool)
			{
				return;
			}
			var pin = new DesignTime().UserPin;
			Title = pin.Name;
			Address = pin.FormattedAddress;
			Icon = pin.Icon;
			VisualState = ExpansionStates.Expanded;
#endif
		}

		#endregion


		#region Commands

		public ICommand CommandRefresh
		{
			get { return (ICommand)GetValue(CommandRefreshProperty); }
			set { SetValue(CommandRefreshProperty, value); }
		}

		public static readonly DependencyProperty CommandRefreshProperty = DependencyProperty.Register(
			"CommandRefresh",
			typeof(ICommand),
			typeof(PushpinContent),
			null);

		public object CommandRefreshParameter
		{
			get { return GetValue(CommandRefreshParameterProperty); }
			set { SetValue(CommandRefreshParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandRefreshParameterProperty = DependencyProperty.Register(
			"CommandRefreshParameter",
			typeof(object),
			typeof(PushpinContent),
			null);

		#endregion


		#region Public Properties

		public ExpansionStates VisualState
		{
			get { return (ExpansionStates)GetValue(VisualStateProperty); }
			set { SetValue(VisualStateProperty, value); }
		}

		public static readonly DependencyProperty VisualStateProperty = DependencyProperty.Register(
			"VisualState",
			typeof(ExpansionStates),
			typeof(PushpinContent),
			new PropertyMetadata(default(ExpansionStates), OnVisualStateChanged));

		public DataStates WorkingState
		{
			get { return (DataStates)GetValue(WorkingStateProperty); }
			set { SetValue(WorkingStateProperty, value); }
		}

		public static readonly DependencyProperty WorkingStateProperty = DependencyProperty.Register(
			"DataState",
			typeof(DataStates),
			typeof(PushpinContent),
			new PropertyMetadata(default(DataStates), OnWorkingStateChanged));

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title",
			typeof(string),
			typeof(PushpinContent),
			null);

		public string Address
		{
			get { return (string)GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		}

		public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
			"Address",
			typeof(string),
			typeof(PushpinContent),
			null);

		public PlaceIcon Icon
		{
			get { return (PlaceIcon)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
			"Icon",
			typeof(PlaceIcon),
			typeof(PushpinContent),
			new PropertyMetadata(default(PlaceIcon)));

		public bool IsSearchResult
		{
			get { return (bool)GetValue(IsSearchResultProperty); }
			set { SetValue(IsSearchResultProperty, value); }
		}

		public static readonly DependencyProperty IsSearchResultProperty = DependencyProperty.Register(
			"IsSearchResult",
			typeof(bool),
			typeof(PushpinContent),
			new PropertyMetadata(false));

		#endregion


		#region Event Handling

		public override void OnApplyTemplate()
		{
			VisualStateManager.GoToState(this, VisualState.ToString(), false);
			VisualStateManager.GoToState(this, WorkingState.ToString(), false);
			base.OnApplyTemplate();
		}

		private static void OnVisualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var content = (PushpinContent)d;
			VisualStateManager.GoToState(content, content.VisualState.ToString(), true);
		}

		private static void OnWorkingStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var content = (PushpinContent)sender;
			VisualStateManager.GoToState(content, content.WorkingState.ToString(), true);
		}

		#endregion
	}
}
