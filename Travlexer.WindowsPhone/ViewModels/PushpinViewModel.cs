using Codify.WindowsPhone.ViewModels;
using Travlexer.WindowsPhone.Infrastructure.Models;

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
		}

		#endregion


		#region Public Properties

		public bool IsSearchResult
		{
			get { return _isSearchResult; }
			set { SetProperty(ref _isSearchResult, value, IsSearchResultProperty); }
		}

		private bool _isSearchResult;
		private const string IsSearchResultProperty = "IsSearchResult";

		public WorkingStates WorkingState
		{
			get { return _workingState; }
			set { SetProperty(ref _workingState, value, WorkingStateProperty); }
		}

		private WorkingStates _workingState;
		private const string WorkingStateProperty = "WorkingState";

		#endregion
	}
}
