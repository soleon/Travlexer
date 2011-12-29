using Travlexer.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the view model of a user pin on the map.
	/// </summary>
	public class UserPinViewModel : DataViewModelBase<UserPin>
	{
		public UserPinViewModel(UserPin data) : base(data) {}
	}
}