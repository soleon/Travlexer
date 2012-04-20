using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codify;
using Microsoft.Phone.Tasks;
using Travlexer.Data;

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
	[TemplatePart(Name = PART_WebSite, Type = typeof(HyperlinkButton))]
	[TemplatePart(Name = PART_ContactNumber, Type = typeof(HyperlinkButton))]
	public class PushpinContent : Control
	{
		#region Constants

		// Visual states and groups name.
		private const string GROUP_VisualStates = "VisualStates";
		private const string STATE_Expanded = "Expanded";
		private const string STATE_Collapsed = "Collapsed";
		private const string GROUP_LoadStates = "LoadStates";
		private const string STATE_Loading = "Loading";
		private const string STATE_Error = "Error";
		private const string STATE_Loaded = "Loaded";

		// Template parts name.
		private const string
			PART_Content = "Content",
			PART_SearchResultIndicator = "SearchResultIndicator",
			PART_Title = "Title",
			PART_Address = "Address",
			PART_GeoCoordinate = "GeoCoordinate",
			PART_WorkingText = "WorkingText",
			PART_WebSite = "WebSite",
			PART_ContactNumber = "ContactNumber";

		#endregion


		#region Private Members

		HyperlinkButton _webSiteLink, _contactNumberLink;

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
			Title = "Kfc: Mercer Street";
			Address = "201 West Mercer Street, Seattle, WA 98119, United States";
			Address = "201 West Mercer Street, Seattle, WA 98119, United States";
			ContactNumber = "+1 206-283-7575";
			Rating = "2.30";
			WebSite = "http://www.kfc.com/";

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


		#region VisualState

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

		#endregion


		#region WorkingState

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

		#endregion


		#region Title

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

		#endregion


		#region Address

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

		#endregion


		#region Icon

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

		#endregion


		#region IsSearchResult

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


		#region ContactNumber

		public string ContactNumber
		{
			get { return (string)GetValue(ContactNumberProperty); }
			set { SetValue(ContactNumberProperty, value); }
		}

		public static readonly DependencyProperty ContactNumberProperty = DependencyProperty.Register(
			"ContactNumber",
			typeof(string),
			typeof(PushpinContent),
			null);

		#endregion


		#region WebSite

		public string WebSite
		{
			get { return (string)GetValue(WebSiteProperty); }
			set { SetValue(WebSiteProperty, value); }
		}

		public static readonly DependencyProperty WebSiteProperty = DependencyProperty.Register(
			"WebSite",
			typeof(string),
			typeof(PushpinContent),
			null);

		#endregion


		#region Rating

		public string Rating
		{
			get { return (string)GetValue(RatingProperty); }
			set { SetValue(RatingProperty, value); }
		}

		public static readonly DependencyProperty RatingProperty = DependencyProperty.Register(
			"Rating",
			typeof(string),
			typeof(PushpinContent),
			null);

		#endregion


		#endregion


		#region Event Handling

		public override void OnApplyTemplate()
		{
			VisualStateManager.GoToState(this, VisualState.ToString(), false);
			VisualStateManager.GoToState(this, WorkingState.ToString(), false);

			_webSiteLink = GetTemplateChild(PART_WebSite) as HyperlinkButton;
			if (_webSiteLink != null)
			{
				_webSiteLink.Click += (s, e) =>
				{
					Uri uri;
					if (Uri.TryCreate(WebSite, UriKind.RelativeOrAbsolute, out uri))
					{
						new WebBrowserTask { Uri = uri }.Show();
					}
					else
					{
						MessageBox.Show("The web site address doesn't seem valid, we could not navigate to it.", "Incorrect Web Site", MessageBoxButton.OK);
					}
				};
			}

			_contactNumberLink = GetTemplateChild(PART_ContactNumber) as HyperlinkButton;
			if (_contactNumberLink != null)
			{
				_contactNumberLink.Click += (s, e) => new PhoneCallTask { DisplayName = Title, PhoneNumber = ContactNumber }.Show();
			}

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
