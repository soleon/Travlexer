using Travelexer.WindowsPhone.Core.Collections;
using Travelexer.WindowsPhone.Core.Commands;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Views;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the view model for <see cref="MapView"/>
	/// </summary>
	public class MapViewModel : ViewModelBase
	{
		#region Private Fields

		private readonly IDataContext _data = Globals.DataContext;

		#endregion


		#region Constructors

		public MapViewModel()
		{
			UserPins = new AdaptedObservableCollection<UserPin, UserPinViewModel>(userPin => new UserPinViewModel(userPin, this), Globals.DataContext.UserPins);
			CommandAddUserPin = new DelegateCommand<Location>(OnAddUserPin);
			CommandToggleUserPinContentState = new DelegateCommand<UserPinViewModel>(OnToggleUserPinContentState);
			CommandCollapseUserPinContent = new DelegateCommand<UserPinViewModel>(OnCollapseUserPinContent);
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection of all user pins.
		/// </summary>
		public AdaptedObservableCollection<UserPin, UserPinViewModel> UserPins { get; private set; }

		#endregion


		#region Commands

		/// <summary>
		/// Gets the command that adds a user pin.
		/// </summary>
		public DelegateCommand<Location> CommandAddUserPin { get; private set; }

		/// <summary>
		/// Gets the command that toggles the pushpin content state.
		/// </summary>
		public DelegateCommand<UserPinViewModel> CommandToggleUserPinContentState { get; private set; }

		/// <summary>
		/// Gets the command that collapses the user pin.
		/// </summary>
		public DelegateCommand<UserPinViewModel> CommandCollapseUserPinContent { get; private set; }

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when <see cref="CommandToggleUserPinContentState"/> is executed.
		/// </summary>
		private void OnToggleUserPinContentState(UserPinViewModel userPin)
		{
			userPin.State = userPin.State == PushpinContentStates.Collapsed ? PushpinContentStates.Expanded : PushpinContentStates.Collapsed;
		}

		/// <summary>
		/// Called when <see cref="CommandAddUserPin"/> is executed.
		/// </summary>
		private void OnAddUserPin(Location location)
		{
			_data.AddNewUserPin(location);
		}

		/// <summary>
		/// Called when <see cref="CommandCollapseUserPinContent"/> is executed.
		/// </summary>
		private void OnCollapseUserPinContent(UserPinViewModel userPin)
		{
			userPin.State = PushpinContentStates.Collapsed;
		}

		#endregion
	}
}
