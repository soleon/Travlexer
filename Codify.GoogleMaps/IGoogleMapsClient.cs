using System;
using Codify.GoogleMaps.Entities;
using Codify.Models;
using RestSharp;

namespace Codify.GoogleMaps
{
	public interface IGoogleMapsClient
	{
		/// <summary>
		/// Gets a list of <see cref="PlaceDetails"/> that can be found at the specified <see cref="LatLng"/>.
		/// </summary>
		/// <param name="location">The geo-location to match for places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void GetPlaces(LatLng location, Action<RestResponse<ListResponse<PlaceDetails>>> callback = null);

		/// <summary>
		/// Gets the places that match the address.
		/// </summary>
		/// <param name="address">The address to match for places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void GetPlaces(string address, Action<RestResponse<ListResponse<PlaceDetails>>> callback = null);

		/// <summary>
		/// Gets the place details.
		/// </summary>
		/// <param name="reference">The reference key of the place.</param>
		/// <param name="callback">The callback to be executed after the process is finished.</param>
		void GetPlaceDetails(string reference, Action<RestResponse<Response<PlaceDetails>>> callback = null);

		/// <summary>
		/// Gets the suggestions based on the input.
		/// </summary>
		/// <param name="center">The geo-coordinate of where the suggestions will be based on.</param>
		/// <param name="input">The input to populate the suggestions.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void GetSuggestions(LatLng center, string input, Action<RestResponse<AutoCompleteResponse>> callback = null);

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="center">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void Search(LatLng center, string input, Action<RestResponse<ListResponse<Place>>> callback = null);

		/// <summary>
		/// Cancels the current get suggestions operation if there is any.
		/// </summary>
		void CancelGetSuggestions();

		/// <summary>
		/// Gets the directions between the origin location and the destination location.
		/// </summary>
		/// <param name="origin">The origin location.</param>
		/// <param name="destination">The destination location.</param>
		/// <param name="mode">The travelling mode.</param>
		/// <param name="method">The routing method.</param>
		/// <param name="unit">The unit to use in displaying the routing information.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void GetDirections(string origin, string destination, TravelMode mode, RouteMethod method, Unit unit, Action<RestResponse<RoutesResponse>> callback);
	}
}