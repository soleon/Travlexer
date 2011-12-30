using System.ComponentModel;
using Travelexer.WindowsPhone.Core.ViewModels;
using Travlexer.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the view model of a user pin on the map.
	/// </summary>
	public class PushpinViewModel : DataViewModelBase<Place>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PushpinViewModel"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="isSearchResult">Determines if this pushpin is a search result.</param>
		/// <param name="parent">The logical parent view model that owns this view model.</param>
		public PushpinViewModel(Place data, bool isSearchResult = false, IViewModel parent = null)
			: base(data, parent)
		{
			IsSearchResult = isSearchResult;
#if DEBUG
			if (!DesignerProperties.IsInDesignTool)
			{
				return;
			}
			VisualState = PushpinContentVisualStates.Expanded;
#endif
		}

		#endregion


		#region Public Properties

		public bool IsSearchResult { get; private set; }

		public PushpinContentVisualStates VisualState
		{
			get { return _visualState; }
			set { SetProperty(ref _visualState, value, VisualStateProperty); }
		}

		private PushpinContentVisualStates _visualState;
		private const string VisualStateProperty = "VisualState";

		public PushpinContentWorkingStates WorkingState
		{
			get { return _workingState; }
			set { SetProperty(ref _workingState, value, WorkingStateProperty); }
		}

		private PushpinContentWorkingStates _workingState;
		private const string WorkingStateProperty = "WorkingState";

		public PushpinHighlightStates HighlightState
		{
			get { return _highlightState; }
			set { SetProperty(ref _highlightState, value, HighlightStateProperty); }
		}
		private PushpinHighlightStates _highlightState;
		private const string HighlightStateProperty = "HighlightState";

		#endregion
	}
}
