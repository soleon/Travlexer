using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Net;
using Codify;
using Codify.Controls.Maps;
using Codify.Extensions;
using Codify.Models;
using Codify.Serialization;
using Codify.Services;
using Codify.Storage;
using RestSharp;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.Infrastructure.Serialization;
using Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps;
using Place = Travlexer.WindowsPhone.Infrastructure.Models.Place;
using PlaceDetails = Travlexer.WindowsPhone.Infrastructure.Models.PlaceDetails;

namespace Travlexer.WindowsPhone.Infrastructure
{
	public static class DataContext
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContext"/> class.
		/// </summary>
		static DataContext()
		{
			MapCenter = new ObservableValue<GeoCoordinate>(new Location());
			MapZoomLevel = new ObservableValue<double>(1D);
			MapBaseLayer = new ObservableValue<GoogleMapsLayer>();
			SearchInput = new ObservableValue<string>();

			Places = new ReadOnlyObservableCollection<Place>(_places);
			MapOverlays = new ObservableCollection<GoogleMapsLayer>();
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection that contains all user pins.
		/// </summary>
		public static ReadOnlyObservableCollection<Place> Places { get; private set; }

		private static readonly ObservableCollection<Place> _places = new ObservableCollection<Place>();
		private const string PlacesProperty = "Places";

		/// <summary>
		/// Gets or sets the map center geo-location.
		/// </summary>
		public static ObservableValue<GeoCoordinate> MapCenter { get; private set; }

		private const string MapCenterProperty = "MapCenter";

		/// <summary>
		/// Gets or sets the map zoom level.
		/// </summary>
		public static ObservableValue<double> MapZoomLevel { get; private set; }

		private const string MapZoomLevelProperty = "MapZoomLevel";

		/// <summary>
		/// Gets or sets the search input.
		/// </summary>
		public static ObservableValue<string> SearchInput { get; private set; }

		private const string SearchInputProperty = "SearchInput";

		/// <summary>
		/// Gets or sets the map base layer.
		/// </summary>
		public static ObservableValue<GoogleMapsLayer> MapBaseLayer { get; private set; }

		private const string MapBaseLayerProperty = "MapBaseLayer";

		/// <summary>
		/// Gets the map overlays.
		/// </summary>
		public static ObservableCollection<GoogleMapsLayer> MapOverlays { get; private set; }

		private const string MapOverlayProperty = "MapOverlay";

		/// <summary>
		/// Gets or sets the google maps client.
		/// </summary>
		public static IGoogleMapsClient GoogleMapsClient
		{
			get { return _googleMapsClient ?? (_googleMapsClient = new GoogleMapsClient()); }
			set { _googleMapsClient = value; }
		}

		private static IGoogleMapsClient _googleMapsClient;

		/// <summary>
		/// Gets or sets the storage provider for saving and loading data.
		/// </summary>
		public static IStorage StorageProvider
		{
			get { return _storageProvider ?? (_storageProvider = new IsolatedStorage()); }
			set { _storageProvider = value; }
		}

		private static IStorage _storageProvider;

		/// <summary>
		/// Gets or sets the serializer for saving and loading data.
		/// </summary>
		public static ISerializer<byte[]> Serializer
		{
			get { return _serializer ?? (_serializer = new BinarySerializer()); }
			set { _serializer = value; }
		}

		private static ISerializer<byte[]> _serializer;

		#endregion


		#region Public Methods

		/// <summary>
		/// Adds a new place.
		/// </summary>
		/// <param name="location">The location of the place.</param>
		public static Place AddNewPlace(Location location)
		{
			var p = new Place(location);
			_places.Add(p);
			if (p.DataState == DataStates.None || p.DataState == DataStates.Error)
			{
				GetPlaceDetails(p);
			}
			return p;
		}

		/// <summary>
		/// Removes the existing place.
		/// </summary>
		/// <param name="place">The place to be removed.</param>
		public static void RemovePlace(Place place)
		{
			_places.Remove(place);
		}

		/// <summary>
		/// Removes all places.
		/// </summary>
		public static void ClearPlaces()
		{
			_places.Clear();
		}

		/// <summary>
		/// Removes all search results.
		/// </summary>
		public static void ClearSearchResults()
		{
			for (var i = _places.Count - 1; i >= 0; i--)
			{
				if (_places[i].IsSearchResult)
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
		public static void GetPlaceInformation(Place place, Action<CallbackEventArgs> callback = null)
		{
			place.DataState = DataStates.Busy;
			ProcessCall<ListResponse<Services.GoogleMaps.PlaceDetails>, List<Services.GoogleMaps.PlaceDetails>>(
				(c, a) => c.GetPlaces(place.Location, a),
				r =>
				{
					var details = r.Result[0];
					if (place.Details == null)
					{
						place.Details = new PlaceDetails { ContactNumber = details.FormattedPhoneNumber };
					}
					else
					{
						place.Details.ContactNumber = details.FormattedPhoneNumber;
					}
					place.FormattedAddress = details.FormattedAddress;
				},
				args =>
				{
					place.DataState = args.Status == CallbackStatus.Successful ? DataStates.Finished : DataStates.Error;
					callback.ExecuteIfNotNull(args);
				});
		}

		/// <summary>
		/// Gets the details for the specified place by its reference key.
		/// </summary>
		/// <param name="place">The place to get the details for.</param>
		/// <param name="callback">The callback to be executed after the process is finished.</param>
		public static void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null)
		{
			if (place.Reference == null)
			{
				GetPlaceInformation(place, callback);
				return;
			}
			place.DataState = DataStates.Busy;
			ProcessCall<Response<Services.GoogleMaps.PlaceDetails>, Services.GoogleMaps.PlaceDetails>(
				(c, r) => c.GetPlaceDetails(place.Reference, r),
				r =>
				{
					var p = r.Result;
					if (place.Details == null)
					{
						place.Details = new PlaceDetails();
					}
					place.Details.ContactNumber = p.FormattedPhoneNumber;
					place.FormattedAddress = p.FormattedAddress;
					place.Name = p.Name;
					place.ViewPort = p.Geometry.ViewPort;
				},
				args =>
				{
					place.DataState = args.Status == CallbackStatus.Successful ? DataStates.Finished : DataStates.Error;
					callback.ExecuteIfNotNull(args);
				});
		}

		/// <summary>
		/// Gets place details by its reference key.
		/// </summary>
		/// <param name="reference">The reference key to the place.</param>
		/// <param name="callback">The callback to be executed when this process is finished.</param>
		public static void GetPlaceDetails(string reference, Action<CallbackEventArgs<Place>> callback = null)
		{
			ProcessCall<Response<Services.GoogleMaps.PlaceDetails>, Services.GoogleMaps.PlaceDetails, Place>(
				(c, r) => c.GetPlaceDetails(reference, r),
				r =>
				{
					ClearSearchResults();
					var place = (Place) r.Result;
					place.IsSearchResult = true;
					_places.Add(place);
					return place;
				},
				callback);
		}

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="baseLocation">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public static void Search(Location baseLocation, string input, Action<CallbackEventArgs<List<Place>>> callback = null)
		{
			ProcessCall<ListResponse<Services.GoogleMaps.Place>, List<Services.GoogleMaps.Place>, List<Place>>(
				(c, r) => c.Search(baseLocation, input, r),
				r =>
				{
					ClearSearchResults();
					var places = r.Result.Count > 10 ? r.Result.Take(10).Select(p => (Place) p).ToList() : r.Result.Select(p => (Place) p).ToList();
					foreach (var p in places)
					{
						p.IsSearchResult = true;
						_places.Add(p);
						if (p.Reference == null)
						{
							continue;
						}
						GetPlaceDetails(p);
					}
					return places;
				},
				args =>
				{
					if (args.Status != CallbackStatus.EmptyResult)
					{
						callback.ExecuteIfNotNull(args);
						return;
					}

					// If search come back with empty result, try using the input as an address to get the first matching place.
					const string defaultSearchName = "Search Result";
					ProcessCall<ListResponse<Services.GoogleMaps.PlaceDetails>, List<Services.GoogleMaps.PlaceDetails>, List<Place>>(
						(c, r) => c.GetPlaces(input, r),
						r =>
						{
							ClearSearchResults();
							var place = (Place) r.Result[0];
							if (string.IsNullOrEmpty(place.Name))
							{
								place.Name = defaultSearchName;
							}
							place.IsSearchResult = true;
							place.DataState = DataStates.Finished;
							_places.Add(place);
							return new List<Place> { place };
						},
						callback);
				});
		}

		/// <summary>
		/// Gets the suggestions based on the input and center location.
		/// </summary>
		/// <param name="location">The center location to bias the suggestion result.</param>
		/// <param name="input">The input to suggest base on.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public static void GetSuggestions(Location location, string input, Action<CallbackEventArgs<List<SearchSuggestion>>> callback = null)
		{
			ProcessCall<AutoCompleteResponse, List<Suggestion>, List<SearchSuggestion>>(
				(c, r) => c.GetSuggestions(location, input, r),
				r => new List<SearchSuggestion>(r.Result.Select(s => (SearchSuggestion) s).ToList()),
				callback);
		}

		/// <summary>
		/// Cancels the current get suggestions operation if there is any.
		/// </summary>
		public static void CancelGetSuggestions()
		{
			GoogleMapsClient.CancelGetSuggestions();
		}

		/// <summary>
		/// Saves the data context to the storage provided by <see cref="StorageProvider"/>.
		/// </summary>
		public static void SaveContext()
		{
			// Save map center.
			StorageProvider.SaveSetting(MapCenterProperty, (Location) MapCenter.Value);

			// Save map zoom level.
			StorageProvider.SaveSetting(MapZoomLevelProperty, MapZoomLevel.Value);

			// Save search input.
			StorageProvider.SaveSetting(SearchInputProperty, SearchInput.Value);

			// Save map base layer.
			StorageProvider.SaveSetting(MapBaseLayerProperty, MapBaseLayer.Value);

			// Save map overlays.
			var overlays = MapOverlays.ToArray();
			StorageProvider.SaveSetting(MapOverlayProperty, overlays);

			// Save places.
			var placeBytes = Serializer.Serialize(_places.ToArray());
			StorageProvider.SaveSetting(PlacesProperty, placeBytes);
		}

		/// <summary>
		/// Loads the data context from the storage provided by <see cref="StorageProvider"/>.
		/// </summary>
		public static void LoadContext()
		{
			// Load map center.
			Location mapCenter;
			if (StorageProvider.TryGetSetting(MapCenterProperty, out mapCenter))
			{
				MapCenter.Value = mapCenter;
			}

			// Load map zoom level.
			double mapZoomLevel;
			if (StorageProvider.TryGetSetting(MapZoomLevelProperty, out mapZoomLevel))
			{
				MapZoomLevel.Value = mapZoomLevel;
			}

			// Load search input.
			string searchInput;
			if (StorageProvider.TryGetSetting(SearchInputProperty, out searchInput))
			{
				SearchInput.Value = searchInput;
			}

			// Load map base layer.
			GoogleMapsLayer mapBaseLayer;
			if (StorageProvider.TryGetSetting(MapBaseLayerProperty, out mapBaseLayer))
			{
				MapBaseLayer.Value = mapBaseLayer;
			}

			// Load map overlays.
			GoogleMapsLayer[] overlays;
			if (StorageProvider.TryGetSetting(MapOverlayProperty, out overlays))
			{
				overlays.ForEach(MapOverlays.Add);
			}

			// Load places.
			byte[] placeBytes;
			Place[] places;
			if (StorageProvider.TryGetSetting(PlacesProperty, out placeBytes) && Serializer.TryDeserialize(placeBytes, out places))
			{
				places.ForEach(_places.Add);
			}
		}

		/// <summary>
		/// Toggles the specified map overlay.
		/// </summary>
		public static void ToggleMapOverlay(GoogleMapsLayer layer)
		{
			if (MapOverlays.Contains(layer))
			{
				MapOverlays.Remove(layer);
			}
			else
			{
				MapOverlays.Add(layer);
			}
		}

		#endregion


		#region Private Methods

		private static void ProcessCall<TResponse, TResult>(Action<IGoogleMapsClient, Action<RestResponse<TResponse>>> callAction, Action<TResponse> processSuccessfulResponse = null, Action<CallbackEventArgs> callback = null)
			where TResponse : class, IResponse<TResult>
			where TResult : class
		{
			callAction(GoogleMapsClient, r =>
			{
				if (r.ResponseStatus == ResponseStatus.Aborted)
				{
					callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.Cancelled));
					return;
				}
				TResponse data = null;
				TResult result;
				if (r.StatusCode != HttpStatusCode.OK ||
				    (data = r.Data) == null ||
				    data.Status != StatusCodes.OK ||
				    (result = data.Result) == null ||
				    (result is IList && ((IList) result).Count == 0))
				{
					var exception = r.ErrorException;
					if (exception != null)
					{
						callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.ServiceException, exception));
					}
					else if (data == null || data.Status != StatusCodes.ZERO_RESULTS)
					{
						callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.Unknown));
					}
					else
					{
						callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.EmptyResult));
					}
					return;
				}
				processSuccessfulResponse.ExecuteIfNotNull(r.Data);
				callback.ExecuteIfNotNull(new CallbackEventArgs());
			});
		}

		private static void ProcessCall<TResponse, TResult, TCallback>(Action<IGoogleMapsClient, Action<RestResponse<TResponse>>> callAction, Func<TResponse, TCallback> processSuccessfulResponse = null, Action<CallbackEventArgs<TCallback>> callback = null)
			where TResponse : class, IResponse<TResult>
			where TResult : class
		{
			callAction(GoogleMapsClient, r =>
			{
				if (r.ResponseStatus == ResponseStatus.Aborted)
				{
					callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.Cancelled));
					return;
				}
				TResponse data = null;
				TResult result;
				if (r.StatusCode != HttpStatusCode.OK ||
				    (data = r.Data) == null ||
				    data.Status != StatusCodes.OK ||
				    (result = data.Result) == null ||
				    (result is IList && ((IList) result).Count == 0))
				{
					var exception = r.ErrorException;
					if (exception != null)
					{
						callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.ServiceException, exception));
					}
					else if (data == null || data.Status != StatusCodes.ZERO_RESULTS)
					{
						callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.Unknown));
					}
					else
					{
						callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.EmptyResult));
					}
					return;
				}
				var callbackResult = processSuccessfulResponse.ExecuteIfNotNull(data);
				callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(callbackResult));
			});
		}

		#endregion
	}
}
