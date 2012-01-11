using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using RestSharp;
using Travlexer.WindowsPhone.Core.Extensions;
using Travlexer.WindowsPhone.Core.Services;
using Travlexer.WindowsPhone.Core.Threading;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services.GoogleMaps;
using Place = Travlexer.WindowsPhone.Models.Place;
using PlaceDetails = Travlexer.WindowsPhone.Models.PlaceDetails;

namespace Travlexer.WindowsPhone
{
	public class DataContext : IDataContext
	{
		#region Private Fields

		private readonly IGoogleMapsClient _googleMapsClient;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContext"/> class.
		/// </summary>
		public DataContext(IGoogleMapsClient googleMapsClient = null)
		{
			Places = new ReadOnlyObservableCollection<Place>(_places);

			_googleMapsClient = googleMapsClient ?? new GoogleMapsClient();
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection that contains all user pins.
		/// </summary>
		public ReadOnlyObservableCollection<Place> Places { get; private set; }

		private readonly ObservableCollection<Place> _places = new ObservableCollection<Place>();

		#endregion


		#region Public Methods

		/// <summary>
		/// Adds a new place.
		/// </summary>
		/// <param name="location">The location of the place.</param>
		public Place AddNewPlace(Location location)
		{
			var p = new Place(location);
			AddNewPlace(p);
			return p;
		}

		/// <summary>
		/// Adds the new place to the global place list.
		/// </summary>
		/// <param name="place">The new place to be added.</param>
		public void AddNewPlace(Place place)
		{
			_places.Add(place);
		}

		/// <summary>
		/// Removes the existing place.
		/// </summary>
		/// <param name="place">The place to be removed.</param>
		public void RemovePlace(Place place)
		{
			_places.Remove(place);
		}

		/// <summary>
		/// Removes all places.
		/// </summary>
		public void ClearPlaces()
		{
			_places.Clear();
		}

		/// <summary>
		/// Removes all search results.
		/// </summary>
		public void ClearSearchResults()
		{
			for (var i = _places.Count - 1; i >= 0; i--)
			{
				if (!_places[i].IsSearchResult)
				{
					_places.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Gets information of the specified <see cref="Place"/>.
		/// </summary>
		/// <param name="place">The place to get the information for.</param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		public void GetPlaceInformation(Place place, Action<CallbackEventArgs> callback = null)
		{
			_googleMapsClient.GetPlaces(place.Location, response =>
			{
				if (HasErrorAndCallback<ListResponse<Services.GoogleMaps.PlaceDetails>, Services.GoogleMaps.PlaceDetails>(response, callback))
				{
					return;
				}
				var details = response.Data.Results.First();
				if (place.Details == null)
				{
					place.Details = new PlaceDetails { ContactNumber = details.FormattedPhoneNumber };
				}
				else
				{
					place.Details.ContactNumber = details.FormattedPhoneNumber;
				}
				place.FormattedAddress = details.FormattedAddress;
				callback.ExecuteIfNotNull(new CallbackEventArgs());
			});
		}

		/// <summary>
		/// Gets the details for the specified place by its reference key.
		/// </summary>
		/// <param name="place">The place to get the details for.</param>
		/// <param name="callback">The callback to be executed after the process is finished.</param>
		public void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null)
		{
			if (place.Reference == null)
			{
				const string message = "Reference of the target place is required in order to get its details.";
				throw new InvalidOperationException(message);
			}
			GetPlaceDetails(place.Reference, args =>
			{
				var p = args.Result;
				if (place.Details == null)
				{
					place.Details = new PlaceDetails();
				}
				place.Details.ContactNumber = p.Details.ContactNumber;
				place.FormattedAddress = p.FormattedAddress;
				place.Name = p.Name;
				place.ViewPort = p.ViewPort;
				callback.ExecuteIfNotNull(new CallbackEventArgs(args.Status, args.Exception));
			});
		}

		/// <summary>
		/// Gets place details by its reference key.
		/// </summary>
		/// <param name="reference">The reference key to the place.</param>
		/// <param name="callback">The callback to be executed when this process is finished.</param>
		public void GetPlaceDetails(string reference, Action<CallbackEventArgs<Place>> callback = null)
		{
			_googleMapsClient.GetPlaceDetails(reference, response =>
			{
				if (HasErrorAndCallback<Response<Services.GoogleMaps.PlaceDetails>, Services.GoogleMaps.PlaceDetails, Place>(response, callback))
				{
					return;
				}

				callback.ExecuteIfNotNull(new CallbackEventArgs<Place>(response.Data.Result));
			});
		}

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="baseLocation">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public void Search(Location baseLocation, string input, Action<CallbackEventArgs<List<Place>>> callback = null)
		{
			_googleMapsClient.Search(baseLocation, input, callback: response =>
			{
				if (HasErrorAndCallback<ListResponse<Services.GoogleMaps.Place>, Services.GoogleMaps.Place, List<Place>>(response, callback))
				{
					return;
				}

				ClearSearchResults();
				var results = response.Data.Results;
				var places = results.Take(10).Select(p => (Place)p).ToList();
				callback.ExecuteIfNotNull(new CallbackEventArgs<List<Place>>(places));
			});
		}

		/// <summary>
		/// Gets the suggestions based on the input and center location.
		/// </summary>
		/// <param name="location">The center location to bias the suggestion result.</param>
		/// <param name="input">The input to suggest base on.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public void GetSuggestions(Location location, string input, Action<CallbackEventArgs<List<SearchSuggestion>>> callback = null)
		{
			_googleMapsClient.GetSuggestions(location, input, response =>
			{
				if (HasErrorAndCallback<AutoCompleteResponse, Suggestion, List<SearchSuggestion>>(response, callback))
				{
					return;
				}
				var suggestions = response.Data.Results;
				var result = new CallbackEventArgs<List<SearchSuggestion>>(suggestions.Select(s => (SearchSuggestion)s).ToList());
				callback.ExecuteIfNotNull(result);
			});
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Callbacks if the response result has any error.
		/// </summary>
		/// <param name="response">The response to check.</param>
		/// <param name="callback">The callback to execute if there's any error.</param>
		/// <returns><c>true</c> if there is any error and the callback has been executed, otherwise, <c>false</c>.</returns>
		private static bool HasErrorAndCallback<TResponseData, TListResult>(RestResponse<TResponseData> response, Action<CallbackEventArgs> callback = null) where TResponseData : class, IResponse
		{
			var responseStatus = response.ResponseStatus;
			if (responseStatus == ResponseStatus.Aborted)
			{
				callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.Cancelled));
				return true;
			}
			TResponseData data;
			if (response.StatusCode != HttpStatusCode.OK ||
				(data = response.Data) == null ||
				data.Status != StatusCodes.OK ||
				(data is IListResponse<TListResult> && ((IListResponse<TListResult>)data).Results.IsNullOrEmpty()))
			{
				var exception = response.ErrorException;
				callback.ExecuteIfNotNull(exception != null ? new CallbackEventArgs(CallbackStatus.ServiceException, exception) : new CallbackEventArgs(CallbackStatus.Unknown));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Callbacks if the response result has any error.
		/// </summary>
		/// <typeparam name="TResponseData">The type data carried by the <see cref="RestResponse{T}"/>.</typeparam>
		/// <typeparam name="TCallback">The type of parameter that the <see cref="CallbackEventArgs{T}"/> carries.</typeparam>
		/// <typeparam name="TListResult">The type of the list items if <see cref="TResponseData"/> is of type <see cref="IListResponse{T}"/>.</typeparam>
		/// <param name="response">The response.</param>
		/// <param name="callback">The callback.</param>
		/// <returns><c>true</c> if there is any error and the callback has been executed, otherwise, <c>false</c>.</returns>
		private static bool HasErrorAndCallback<TResponseData, TListResult, TCallback>(RestResponse<TResponseData> response, Action<CallbackEventArgs<TCallback>> callback = null) where TResponseData : class, IResponse
		{
			var responseStatus = response.ResponseStatus;
			if (responseStatus == ResponseStatus.Aborted)
			{
				callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.Cancelled));
				return true;
			}
			TResponseData data;
			if (response.StatusCode != HttpStatusCode.OK ||
				(data = response.Data) == null ||
				data.Status != StatusCodes.OK ||
				(data is IListResponse<TListResult> && ((IListResponse<TListResult>)data).Results.IsNullOrEmpty()))
			{
				var exception = response.ErrorException;
				callback.ExecuteIfNotNull(exception != null ? new CallbackEventArgs<TCallback>(CallbackStatus.ServiceException, exception) : new CallbackEventArgs<TCallback>(CallbackStatus.Unknown));
				return true;
			}
			return false;
		}

		#endregion
	}
}
