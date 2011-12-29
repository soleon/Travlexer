using System.Collections.ObjectModel;
using Travlexer.WindowsPhone.Commands;
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

		private IDataContext _data = Globals.DataContext;

		#endregion

		#region Constructors

		public MapViewModel()
		{
			CommandAddUserPin = new DelegateCommand<Location>(OnAddUserPin);
		}

		private void OnAddUserPin(Location location)
		{
			_data.AddNewUserPin(location);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the collection of all user pins.
		/// </summary>
		public ReadOnlyObservableCollection<UserPin> UserPins
		{
			get { return Globals.DataContext.UserPins; }
		}

		#endregion

		#region Commands

		/// <summary>
		/// Gets the command that adds a user pin.
		/// </summary>
		public DelegateCommand<Location> CommandAddUserPin { get; private set; }

		#endregion
	}
}
