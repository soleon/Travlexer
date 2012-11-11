using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Codify.Extensions;
using Codify.GoogleMaps.Entities;
using Codify.Serialization;
using RestSharp;

namespace Codify.GoogleMaps
{
	public class GoogleMapsClient : IGoogleMapsClient
	{
		#region Private Members

		private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

		private const string BaseApiUrl = "https://maps.googleapis.com/maps/api";
		private const string ApiKey = "AIzaSyBEPZ46IhE3WI-7mBtSOkixMr6GSwRgrws";
		private readonly string _language;
		private readonly string _region;
		private readonly string _basePlacesSearchUrl;
		private readonly string _basePlaceDetailsUrl;
		private readonly string _baseGeocodingUrl;
		private readonly string _baseDirectionsUrl;
		private readonly string _baseAutoCompleteUrl;

	    // Global REST request handles for aborting the requests when needed.
		private RestRequestAsyncHandle
			_getSuggestionsAsyncHandle,
			_searchAsyncHandle,
			_getDirectionsAsyncHandle;

		// Static mapping dictionaries.
		private static readonly Dictionary<TravelMode, string> RouteModes = new Dictionary<TravelMode, string>
		{
			{TravelMode.Driving, "driving"},
			{TravelMode.Walking, "walking"},
			{TravelMode.Bicycling, "bicycling"}
		};

		private static readonly Dictionary<RouteMethod, string> RouteMethods = new Dictionary<RouteMethod, string>
		{
			{RouteMethod.Default, null},
			{RouteMethod.AvoidTolls, "tolls"},
			{RouteMethod.AvoidHighways, "highways"}
		};

		private static readonly Dictionary<Units, string> Units = new Dictionary<Units, string>
		{
			{Entities.Units.Imperial, "imperial"},
			{Entities.Units.Metric, "metric"}
		};

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleMapsClient"/> class.
		/// </summary>
		public GoogleMapsClient()
		{
			_language = Utilities.CurrentLanguageCode;

			_region = RegionInfo.CurrentRegion.TwoLetterISORegionName;
			_basePlacesSearchUrl = "place/search/json?radius=5000&sensor=true&key=" + ApiKey + "&language=" + _language;
			_basePlaceDetailsUrl = "place/details/json?sensor=true&key=" + ApiKey + "&language=" + _language + "&reference=";
			_baseGeocodingUrl = "geocode/json?sensor=true&language=" + _language + "&region=" + _region;
			_baseDirectionsUrl = "directions/json?sensor=true&language=" + _language + "&region=" + _region;
			_baseAutoCompleteUrl = "place/autocomplete/json?sensor=true&key=" + ApiKey + "&language=" + _language;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Gets a list of <see cref="Place"/> that can be found at the specified <see cref="LatLng"/>.
		/// </summary>
		/// <param name="location">The geo-location to match for places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public void GetPlaces(LatLng location, Action<IRestResponse<ListResponse<Place>>> callback = null)
		{
			ProcessRequest<ListResponse<Place>, List<Place>>(
				new RestRequest(_baseGeocodingUrl + "&latlng=" + location),
				response => response.Data = JsonSerializer.Deserialize<ListResponse<Place>>(response.Content),
				callback: callback);
		}

		/// <summary>
		/// Gets the places that match the address.
		/// </summary>
		/// <param name="address">The address to match for places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
        public void GetPlaces(string address, Action<IRestResponse<ListResponse<Place>>> callback = null)
		{
			ProcessRequest<ListResponse<Place>, List<Place>>(
				new RestRequest(_baseGeocodingUrl + "&address=" + address),
				r => r.Data = JsonSerializer.Deserialize<ListResponse<Place>>(r.Content),
				callback: callback);
		}

		/// <summary>
		/// Gets the place details.
		/// </summary>
		/// <param name="reference">The reference key of the place.</param>
		/// <param name="callback">The callback to be executed after the process is finished.</param>
        public void GetPlaceDetails(string reference, Action<IRestResponse<Response<Place>>> callback = null)
		{
			ProcessRequest<Response<Place>, Place>(
				new RestRequest(_basePlaceDetailsUrl + reference),
				r => r.Data = JsonSerializer.Deserialize<Response<Place>>(r.Content),
				callback: callback);
		}

		/// <summary>
		/// Gets the suggestions based on the input.
		/// </summary>
		/// <param name="center">The geo-coordinate of where the suggestions will be based on.</param>
		/// <param name="input">The input to populate the suggestions.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
        public void GetSuggestions(LatLng center, string input, Action<IRestResponse<AutoCompleteResponse>> callback = null)
		{
			// Ensure single effective request.
			CancelGetSuggestions();

			_getSuggestionsAsyncHandle = ProcessRequest<AutoCompleteResponse, List<Suggestion>>(
				new RestRequest(_baseAutoCompleteUrl + "&location=" + center + "&input=" + input),
				r => r.Data = JsonSerializer.Deserialize<AutoCompleteResponse>(r.Content),
				callback: r =>
				{
					_getSuggestionsAsyncHandle = null;
					callback.ExecuteIfNotNull(r);
				});
		}

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="center">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
        public void Search(LatLng center, string input, Action<IRestResponse<ListResponse<Place>>> callback = null)
		{
			// Ensure single effective request.
			_searchAsyncHandle.UseIfNotNull(handle => handle.Abort());

			_searchAsyncHandle = ProcessRequest<ListResponse<Place>, List<Place>>(
				new RestRequest(_basePlacesSearchUrl + "&location=" + center + "&keyword=" + HttpUtility.UrlEncode(input)),
				r => r.Data = JsonSerializer.Deserialize<ListResponse<Place>>(r.Content),
				callback: r =>
				{
					_searchAsyncHandle = null;
					callback.ExecuteIfNotNull(r);
				});
		}

		/// <summary>
		/// Cancels the current get suggestions operation if there is any.
		/// </summary>
		public void CancelGetSuggestions()
		{
			_getSuggestionsAsyncHandle.UseIfNotNull(h=>h.Abort());
		}

		/// <summary>
		/// Gets the directions between the origin location and the destination location.
		/// </summary>
		/// <param name="origin">The origin location.</param>
		/// <param name="destination">The destination location.</param>
		/// <param name="mode">The travelling mode.</param>
		/// <param name="method">The routing method.</param>
		/// <param name="unit">The unit to use in displaying the routing information.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
        public void GetDirections(string origin, string destination, TravelMode mode, RouteMethod method, Units unit, Action<IRestResponse<RoutesResponse>> callback)
		{
			// Ensure single effective request.
			_getDirectionsAsyncHandle.UseIfNotNull(h => h.Abort());

			var routeMode = RouteModes[mode];
			var routeMethod = RouteMethods[method];
			var routeUnit = Units[unit];
			_getDirectionsAsyncHandle = ProcessRequest<RoutesResponse, List<Route>>(
				new RestRequest(_baseDirectionsUrl + "&origin=" + origin + "&destination=" + destination + "&mode=" + routeMode + (routeMethod == null ? null : "&avoid=" + routeMethod) + "&units=" + routeUnit),
				r => r.Data = JsonSerializer.Deserialize<RoutesResponse>(r.Content),
				callback: r =>
				{
					_getDirectionsAsyncHandle = null;
					callback.ExecuteIfNotNull(r);
				});
		}


		#endregion


		#region Private Methods

		/// <summary>
		/// Processes an async REST request.
		/// </summary>
		/// <typeparam name="TResponse">Type of the response data.</typeparam>
		/// <typeparam name="TResult">Type of the result in the response data.</typeparam>
		/// <param name="request">The request to be processed.</param>
		/// <param name="failedResponseAction">The action to be executed if the request came back with a failed response.</param>
		/// <param name="validateResponse">An optional function to validate response for special validation requirement.</param>
		/// <param name="callback">The final callback to be executed.</param>
		/// <returns>The <see cref="RestSharp.RestRequestAsyncHandle"/> associated with this request.</returns>
		private static RestRequestAsyncHandle ProcessRequest<TResponse, TResult>(IRestRequest request, Action<IRestResponse<TResponse>> failedResponseAction = null, Func<IRestResponse<TResponse>, bool> validateResponse = null, Action<IRestResponse<TResponse>> callback = null)
			where TResponse : class, IResponse<TResult>, new()
			where TResult : class
		{
			var c = new RestClient(BaseApiUrl);
			return c.ExecuteAsync<TResponse>(request, r =>
			{
				TResponse data;
				if (r == null || r.StatusCode != HttpStatusCode.OK || (data = r.Data) == null || data.Result == null || r.Content.IsNullOrEmpty() || !validateResponse.ExecuteIfNotNull(r, true))
				{
					try
					{
						failedResponseAction.ExecuteIfNotNull(r);
					}
					catch { }
				}
				callback.ExecuteIfNotNull(r);
			});
		}

		#endregion
	}
}
