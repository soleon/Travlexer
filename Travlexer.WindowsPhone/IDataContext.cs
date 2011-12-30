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
		/// Gets the collection that contains all places shown on the map.
		/// </summary>
		ReadOnlyObservableCollection<Place> Places { get; }

		#endregion


		#region Methods

		/// <summary>
		/// Adds a new place.
		/// </summary>
		/// <param name="location">The location of the place.</param>
		/// <param name="icon">The icon of the place.</param>
		/// <param name="callback">The action to execute after the process is finished.</param>
		Place AddNewPlace(Location location, PlaceIcon icon = default(PlaceIcon), Action<CallbackEventArgs> callback = null);

		/// <summary>
		/// Removes the existing place.
		/// </summary>
		/// <param name="place">The place to be removed.</param>
		void RemovePlace(Place place);

		/// <summary>
		/// Removes all places.
		/// </summary>
		void ClearPlaces();

		/// <summary>
		/// Removes all places that are not pinned.
		/// </summary>
		void ClearUnPinnedPlaces();

		/// <summary>
		/// Gets a list of <see cref="PlaceDetails"/> for the specified location.
		/// </summary>
		/// <param name="place"></param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null);

		#endregion
	}
}
