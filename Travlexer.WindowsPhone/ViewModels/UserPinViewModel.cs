using System.ComponentModel;
using Travlexer.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the view model of a user pin on the map.
	/// </summary>
	public class UserPinViewModel : DataViewModelBase<UserPin>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UserPinViewModel"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="parent">The logical parent view model that owns this view model.</param>
		public UserPinViewModel(UserPin data, IViewModel parent = null)
			: base(data, parent)
		{
#if DEBUG
			if (!DesignerProperties.IsInDesignTool)
			{
				return;
			}
			State = PushpinContentStates.Expanded;
#endif
		}

		#endregion


		#region Public Properties

		public PushpinContentStates State
		{
			get { return _state; }
			set { SetProperty(ref _state, value, StateProperty); }
		}

		private PushpinContentStates _state;
		private const string StateProperty = "State";

		#endregion
	}
}
