using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Travlexer.WindowsPhone.Core.Services;
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
		/// Adds the new place to the global place list.
		/// </summary>
		/// <param name="place">The new place to be added.</param>
		void AddNewPlace(Place place);

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
		void ClearSearchResults();

		/// <summary>
		/// Gets information of the specified <see cref="Place"/>.
		/// </summary>
		/// <param name="place">The place to get the information for.</param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		void GetPlaceInformation(Place place, Action<CallbackEventArgs> callback = null);

		/// <summary>
		/// Gets the details for the specified place.
		/// </summary>
		/// <param name="place">The place to get the details for.</param>
		/// <param name="callback">The callback to be executed after the process is finished.</param>
		void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null);

		/// <summary>
		/// Adds a place by its reference key.
		/// </summary>
		/// <param name="reference">The reference key to the place.</param>
		/// <param name="callback">The callback to be executed when this process is finished.</param>
		void GetPlaceDetails(string reference, Action<CallbackEventArgs<Place>> callback = null);

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="baseLocation">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void Search(Location baseLocation, string input, Action<CallbackEventArgs<List<Place>>> callback = null);

		/// <summary>
		/// Gets the suggestions based on the input and center location.
		/// </summary>
		/// <param name="location">The center location to bias the suggestion result.</param>
		/// <param name="input">The input to suggest base on.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void GetSuggestions(Location location, string input, Action<CallbackEventArgs<List<SearchSuggestion>>> callback = null);

		#endregion
	}
}
