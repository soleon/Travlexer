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
		/// <param name="parent">The logical parent view model that owns this view model.</param>
		public PushpinViewModel(Place data, IViewModel parent = null)
			: base(data, parent) {}

		#endregion


		#region Public Properties

		public WorkingStates WorkingState
		{
			get { return _workingState; }
			set { SetProperty(ref _workingState, value, WorkingStateProperty); }
		}

		private WorkingStates _workingState;
		private const string WorkingStateProperty = "WorkingState";

		/// <summary>
		/// Gets or sets a value indicating whether this instance is being dragged.
		/// </summary>
		public bool IsDragging
		{
			get { return _isDragging; }
			set { SetProperty(ref _isDragging, value, IsDraggingProperty); }
		}

		private bool _isDragging;
		private const string IsDraggingProperty = "IsDragging";

		#endregion
	}
}
