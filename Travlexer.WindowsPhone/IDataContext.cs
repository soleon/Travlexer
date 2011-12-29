using System;
using System.Collections.ObjectModel;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services;

namespace Travlexer.WindowsPhone
{
	/// <summary>
	/// Defines the only data contract for external sources to access and manipulate data.
	/// </summary>
	public interface IDataContext
	{
		#region Properties

		/// <summary>
		/// Gets the collection that contains all user pins.
		/// </summary>
		ReadOnlyObservableCollection<UserPin> UserPins { get; }

		/// <summary>
		/// Gets the collection that contains all search results.
		/// </summary>
		ReadOnlyObservableCollection<SearchResult> SearchResults { get; }

		#endregion


		#region Methods

		/// <summary>
		/// Adds a new user pin.
		/// </summary>
		/// <param name="icon">The icon of the user pin.</param>
		/// <param name="location">The location of the user pin.</param>
		void AddNewUserPin(Location location, PlaceIcon icon = default(PlaceIcon));

		/// <summary>
		/// Removes the existing user pin.
		/// </summary>
		/// <param name="userPin">The pin.</param>
		void RemovePin(UserPin userPin);

		/// <summary>
		/// Removes all user pins.
		/// </summary>
		void ClearUserPins();

		/// <summary>
		/// Removes the search result.
		/// </summary>
		/// <param name="result">The result to be removed.</param>
		void RemoveSearchResult(SearchResult result);

		/// <summary>
		/// Removes all search results.
		/// </summary>
		void ClearSearchResults();

		/// <summary>
		/// Gets a list of <see cref="PlaceDetails"/> for the specified location.
		/// </summary>
		/// <param name="place"></param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null);

		#endregion
	}
}
