using System;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls.Maps;
using Travelexer.WindowsPhone.Core.Services;
using Travlexer.WindowsPhone.Models;

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
		Place AddNewPlace(Location location);

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
		/// Gets information of the specified <see cref="Place"/>.
		/// </summary>
		/// <param name="place">The place to get the information for.</param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		void GetPlaceInformation(Place place, Action<CallbackEventArgs> callback = null);

		#endregion
	}
}
