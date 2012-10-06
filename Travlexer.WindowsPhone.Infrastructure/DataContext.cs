using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Reflection;
using Codify;
using Codify.Entities;
using Codify.Extensions;
using Codify.GoogleMaps;
using Codify.GoogleMaps.Controls;
using Codify.GoogleMaps.Entities;
using Codify.Serialization;
using Codify.Services;
using Codify.Storage;
using RestSharp;
using Travlexer.Data;
using Place = Travlexer.Data.Place;
using Route = Travlexer.Data.Route;
using RouteMethod = Travlexer.Data.RouteMethod;
using TravelMode = Travlexer.Data.TravelMode;

namespace Travlexer.WindowsPhone.Infrastructure
{
    public class DataContext : IDataContext
    {
        #region Private Fields

        private readonly ISerializer<byte[]> _binarySerializer;
        private readonly IGoogleMapsClient _googleMapsClient;
        private readonly IStorage _storageProvider;

        #endregion


        #region Public Events

        /// <summary>
        /// Occurs when <see cref="Places"/> collection is changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler PlacesCollectionChanged
        {
            add { _places.CollectionChanged += value; }
            remove { _places.CollectionChanged -= value; }
        }

        #endregion


        #region Public Properties

        /// <summary>
        ///   Gets the collection that contains all user pins.
        /// </summary>
        public ReadOnlyObservableCollection<Place> Places { get; private set; }

        private readonly ObservableCollection<Place> _places;
        private const string PlacesProperty = "Places";

        /// <summary>
        ///   Gets or sets the selected place.
        /// </summary>
        public ObservableValue<Place> SelectedPlace { get; set; }

        private const string SelectedPlaceProperty = "SelectedPlace";

        /// <summary>
        /// Gets or sets the selected route.
        /// </summary>
        public ObservableValue<Route> SelectedRoute { get; set; }

        private const string SelectedRouteProperty = "SelectedRoute";

        /// <summary>
        ///   Gets or sets the map center geo-location.
        /// </summary>
        public ObservableValue<GeoCoordinate> MapCenter { get; private set; }

        private const string MapCenterProperty = "MapCenter";

        /// <summary>
        ///   Gets or sets the map zoom level.
        /// </summary>
        public ObservableValue<double> MapZoomLevel { get; private set; }

        private const string MapZoomLevelProperty = "MapZoomLevel";

        /// <summary>
        ///   Gets or sets the search input.
        /// </summary>
        public ObservableValue<string> SearchInput { get; private set; }

        private const string SearchInputProperty = "SearchInput";

        /// <summary>
        ///   Gets the route method.
        /// </summary>
        public ObservableValue<RouteMethod> RouteMethod { get; private set; }

        private const string RouteMethodProperty = "RouteMethod";

        /// <summary>
        ///   Gets the route mode.
        /// </summary>
        public ObservableValue<TravelMode> TravelMode { get; private set; }

        private const string TravelModeProperty = "TravelMode";

        /// <summary>
        ///   Gets or sets the map base layer.
        /// </summary>
        public ObservableValue<Layer> MapBaseLayer { get; private set; }

        private const string MapBaseLayerProperty = "MapBaseLayer";

        /// <summary>
        ///   Gets the map overlays.
        /// </summary>
        public ObservableCollection<Layer> MapOverlays { get; private set; }

        private const string MapOverlayProperty = "MapOverlay";

        /// <summary>
        ///   Gets the collection of all routes planned by the user.
        /// </summary>
        public ReadOnlyObservableCollection<Route> Routes { get; private set; }

        private readonly ObservableCollection<Route> _routes;
        private const string RoutesProperty = "Routes";

        /// <summary>
        ///   Gets the collection of all trips planned by the user.
        /// </summary>
        public ReadOnlyObservableCollection<Trip> Trips { get; private set; }

        private readonly ObservableCollection<Trip> _trips;
        private const string TripsProperty = "Trips";

        /// <summary>
        ///   Gets the unit system that is currently in use.
        /// </summary>
        public ObservableValue<Units> Unit { get; private set; }

        private const string UnitProperty = "Unit";

        /// <summary>
        ///   Gets a dictionary that contains available place icon enums mapping to their display names.
        /// </summary>
        public Dictionary<PlaceIcon, string> PlaceIconMap
        {
            get { return _placeIconMap; }
        }

        private static readonly Dictionary<PlaceIcon, string> _placeIconMap;

        /// <summary>
        ///   Gets a dictionary that contains available element color enums mapping to their display names.
        /// </summary>
        public Dictionary<ElementColor, string> ElementColorMap
        {
            get { return _elementColorMap; }
        }

        private static readonly Dictionary<ElementColor, string> _elementColorMap;

        #endregion


        #region Public Methods

        /// <summary>
        ///   Adds a new place.
        /// </summary>
        /// <param name="location"> The location of the place. </param>
        public Place AddNewPlace(Location location)
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
        ///   Removes the existing place.
        /// </summary>
        /// <param name="place"> The place to be removed. </param>
        public void RemovePlace(Place place)
        {
            var connectedRouteIds = place.ConnectedRouteIds;
            if (connectedRouteIds.Count > 0)
                _routes.Where(r => connectedRouteIds.Contains(r.Id)).ToArray().ForEach(RemoveRoute);
            _places.Remove(place);
        }

        /// <summary>
        ///   Removes the place specified by the id.
        /// </summary>
        /// <param name="id"> The id of the place to be removed. </param>
        public void RemovePlace(Guid id)
        {
            _places.FirstOrDefault(p => p.Id == id).UseIfNotNull(p => _places.Remove(p));
        }

        /// <summary>
        ///   Removes the specified route.
        /// </summary>
        public void RemoveRoute(Route route)
        {
            var id = route.Id;
            _places.ForEach(p => p.ConnectedRouteIds.Remove(id));
            _routes.Remove(route);
        }

        /// <summary>
        ///   Removes all places.
        /// </summary>
        public void ClearPlaces()
        {
            _places.Clear();
        }

        /// <summary>
        ///   Removes all search results.
        /// </summary>
        public void ClearSearchResults()
        {
            for (var i = _places.Count - 1; i >= 0; i--)
                if (_places[i].IsSearchResult)
                    _places.RemoveAt(i);
        }

        /// <summary>
        ///   Gets information of the specified <see cref="Travlexer.Data.Place" />.
        /// </summary>
        /// <param name="place"> The place to get the information for. </param>
        /// <param name="callback"> The callback to be executed after this process is finished. </param>
        public void GetPlaceInformation(Place place, Action<CallbackEventArgs> callback = null)
        {
            place.DataState = DataStates.Busy;
            ProcessCall<ListResponse<Codify.GoogleMaps.Entities.Place>, List<Codify.GoogleMaps.Entities.Place>>(
                (c, a) => c.GetPlaces(place.Location.ToGoogleLocation(), a),
                r =>
                {
                    var result = r.Result[0];
                    place.ContactNumber = result.InternationalPhoneNumber ?? result.FormattedPhoneNumber;
                    place.Address = result.FormattedAddress;
                    place.ViewPort = result.Geometry.ViewPort.ToLocalViewPort();
                    place.Reference = result.Reference;
                    place.WebSite = result.WebSite;
                    place.Rating = result.Raiting;

                    if (!string.IsNullOrEmpty(result.Name))
                    {
                        place.Name = result.Name;
                    }
                },
                args =>
                {
                    place.DataState = args.Status == CallbackStatus.Successful ? DataStates.Finished : DataStates.Error;
                    callback.ExecuteIfNotNull(args);
                });
        }

        /// <summary>
        ///   Gets information of the specified <see cref="Travlexer.Data.Place" />.
        /// </summary>
        /// <param name="location"> The geo-location to get the information for. </param>
        /// <param name="callback"> The callback to be executed after this process is finished. </param>
        public void GetAddress(Location location, Action<CallbackEventArgs<string>> callback = null)
        {
            ProcessCall<ListResponse<Codify.GoogleMaps.Entities.Place>, List<Codify.GoogleMaps.Entities.Place>, string>(
                (c, a) => c.GetPlaces(location.ToGoogleLocation(), a),
                r =>
                {
                    var details = r.Result[0];
                    return details.FormattedAddress;
                },
                callback);
        }

        /// <summary>
        ///   Gets the details for the specified place by its reference key.
        /// </summary>
        /// <param name="place"> The place to get the details for. </param>
        /// <param name="callback"> The callback to be executed after the process is finished. </param>
        public void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null)
        {
            if (place.Reference == null)
            {
                GetPlaceInformation(place, callback);
                return;
            }
            place.DataState = DataStates.Busy;
            ProcessCall<Response<Codify.GoogleMaps.Entities.Place>, Codify.GoogleMaps.Entities.Place>(
                (c, r) => c.GetPlaceDetails(place.Reference, r),
                r =>
                {
                    var result = r.Result;
                    place.ContactNumber = result.InternationalPhoneNumber ?? result.FormattedPhoneNumber;
                    place.Address = result.FormattedAddress;
                    place.ViewPort = result.Geometry.ViewPort.ToLocalViewPort();
                    place.Reference = result.Reference;
                    place.WebSite = result.WebSite;
                    place.Rating = result.Raiting;

                    if (!string.IsNullOrEmpty(result.Name))
                    {
                        place.Name = result.Name;
                    }
                },
                args =>
                {
                    place.DataState = args.Status == CallbackStatus.Successful ? DataStates.Finished : DataStates.Error;
                    callback.ExecuteIfNotNull(args);
                });
        }

        /// <summary>
        ///   Gets place details by its reference key.
        /// </summary>
        /// <param name="reference"> The reference key to the place. </param>
        /// <param name="callback"> The callback to be executed when this process is finished. </param>
        public void GetPlaceDetailsForSearch(string reference, Action<CallbackEventArgs<Place>> callback = null)
        {
            ProcessCall<Response<Codify.GoogleMaps.Entities.Place>, Codify.GoogleMaps.Entities.Place, Place>(
                (c, r) => c.GetPlaceDetails(reference, r),
                r =>
                {
                    ClearSearchResults();
                    var place = r.Result.ToPlace();
                    place.IsSearchResult = true;
                    _places.Add(place);
                    return place;
                },
                callback);
        }

        /// <summary>
        ///   Searches for places that matches the input.
        /// </summary>
        /// <param name="baseLocation"> The geo-coordinate around which to retrieve place information. </param>
        /// <param name="input"> The input to search places. </param>
        /// <param name="callback"> The callback to execute after the process is finished. </param>
        public void Search(Location baseLocation, string input, Action<CallbackEventArgs<List<Place>>> callback = null)
        {
            ProcessCall<ListResponse<Codify.GoogleMaps.Entities.Place>, List<Codify.GoogleMaps.Entities.Place>, List<Place>>(
                (c, r) => c.Search(baseLocation.ToGoogleLocation(), input, r),
                r =>
                {
                    ClearSearchResults();
                    var places = r.Result.Count > 10 ? r.Result.Take(10).Select(p => p.ToPlace()).ToList() : r.Result.Select(p => p.ToPlace()).ToList();
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
                    ProcessCall<ListResponse<Codify.GoogleMaps.Entities.Place>, List<Codify.GoogleMaps.Entities.Place>, List<Place>>(
                        (c, r) => c.GetPlaces(input, r),
                        r =>
                        {
                            ClearSearchResults();
                            var place = r.Result[0].ToPlace();
                            if (string.IsNullOrEmpty(place.Name))
                                place.Name = defaultSearchName;
                            place.IsSearchResult = true;
                            place.DataState = DataStates.Finished;
                            _places.Add(place);
                            return new List<Place> { place };
                        },
                        callback);
                });
        }

        /// <summary>
        ///   Gets the suggestions based on the input and center location.
        /// </summary>
        /// <param name="location"> The center location to bias the suggestion result. </param>
        /// <param name="input"> The input to suggest base on. </param>
        /// <param name="callback"> The callback to execute after the process is finished. </param>
        public void GetSuggestions(Location location, string input, Action<CallbackEventArgs<List<SearchSuggestion>>> callback = null)
        {
            ProcessCall<AutoCompleteResponse, List<Suggestion>, List<SearchSuggestion>>(
                (c, r) => c.GetSuggestions(location.ToGoogleLocation(), input, r),
                r => new List<SearchSuggestion>(r.Result.Select(s => s.ToLocalSearchSuggestion()).ToList()),
                callback);
        }

        /// <summary>
        ///   Cancels the current get suggestions operation if there is any.
        /// </summary>
        public void CancelGetSuggestions()
        {
            _googleMapsClient.CancelGetSuggestions();
        }

        /// <summary>
        ///   Finds the route between the depart location and arrive location.
        /// </summary>
        /// <param name="departure"> The depart location. </param>
        /// <param name="arrival"> The arrive location. </param>
        /// <param name="mode"> The travel mode for the route. </param>
        /// <param name="method"> The routing method for the route. </param>
        /// <param name="callback"> The callback to execute after the process is finished. </param>
        public void GetRoute(string departure, string arrival, TravelMode mode, RouteMethod method, Action<CallbackEventArgs<Route>> callback = null)
        {
            ProcessCall<RoutesResponse, List<Codify.GoogleMaps.Entities.Route>, Route>(
                (c, r) => c.GetDirections(departure, arrival, mode.ToGoogleTravelMode(), method.ToGoogleRouteMethod(), Unit.Value.ToGoogleUnits(), r),
                response =>
                {
                    var googleRoute = response.Result.FirstOrDefault();
                    Route existingRoute = null;
                    if (googleRoute != null)
                    {
                        var route = googleRoute.ToLocalRoute();
                        if (route != null && (existingRoute = _routes.FirstOrDefault(r => r == route)) == null)
                        {
                            _routes.Add(route);
                            return route;
                        }
                    }
                    return existingRoute;
                },
                callback);
        }

        /// <summary>
        ///   Clears all routes.
        /// </summary>
        public void ClearRoutes()
        {
            _routes.Clear();
        }

        /// <summary>
        ///   Saves the data context to the storage provided by <see cref="_storageProvider" />.
        /// </summary>
        public void SaveContext()
        {
            // Save map center.
            _storageProvider.SaveSetting(MapCenterProperty, MapCenter.Value.ToLocalLocation());

            // Save map zoom level.
            _storageProvider.SaveSetting(MapZoomLevelProperty, MapZoomLevel.Value);

            // Save search input.
            _storageProvider.SaveSetting(SearchInputProperty, SearchInput.Value);

            // Save map base layer.
            _storageProvider.SaveSetting(MapBaseLayerProperty, MapBaseLayer.Value);

            // Save map overlays.
            var overlays = MapOverlays.ToArray();
            _storageProvider.SaveSetting(MapOverlayProperty, overlays);

            // Save places.
            var placeBytes = _binarySerializer.Serialize(_places.ToArray());
            _storageProvider.SaveSetting(PlacesProperty, placeBytes);

            // Save routes.
            var routeBytes = _binarySerializer.Serialize(_routes.ToArray());
            _storageProvider.SaveSetting(RoutesProperty, routeBytes);

            // Save route method.
            _storageProvider.SaveSetting(RouteMethodProperty, RouteMethod.Value);

            // Save travel mode.
            _storageProvider.SaveSetting(TravelModeProperty, TravelMode.Value);

            // Save selected place ID.
            _storageProvider.SaveSetting(SelectedPlaceProperty, SelectedPlace.Value.UseIfNotNull(p => p.Id));

            // Save selected route ID.
            _storageProvider.SaveSetting(SelectedRouteProperty, SelectedRoute.Value.UseIfNotNull(r => r.Id));
        }

        /// <summary>
        ///   Loads the data context from the storage provided by <see cref="_storageProvider" />.
        /// </summary>
        public void LoadContext()
        {
            // Load map center.
            Location mapCenter;
            if (_storageProvider.TryGetSetting(MapCenterProperty, out mapCenter))
                MapCenter.Value = mapCenter.ToGeoCoordinate();

            // Load map zoom level.
            double mapZoomLevel;
            if (_storageProvider.TryGetSetting(MapZoomLevelProperty, out mapZoomLevel))
                MapZoomLevel.Value = mapZoomLevel;

            // Load search input.
            string searchInput;
            if (_storageProvider.TryGetSetting(SearchInputProperty, out searchInput))
                SearchInput.Value = searchInput;

            // Load map base layer.
            Layer mapBaseLayer;
            if (_storageProvider.TryGetSetting(MapBaseLayerProperty, out mapBaseLayer))
                MapBaseLayer.Value = mapBaseLayer;

            // Load map overlays.
            Layer[] overlays;
            if (_storageProvider.TryGetSetting(MapOverlayProperty, out overlays))
                overlays.ForEach(MapOverlays.Add);

            // Load places.
            byte[] placeBytes;
            Place[] places;
            if (_storageProvider.TryGetSetting(PlacesProperty, out placeBytes) && _binarySerializer.TryDeserialize(placeBytes, out places))
                _places.AddRange(places);

            // Load routes.
            byte[] routeBytes;
            Route[] routes;
            if (_storageProvider.TryGetSetting(RoutesProperty, out routeBytes) && _binarySerializer.TryDeserialize(routeBytes, out routes))
                _routes.AddRange(routes);

            // Load route method.
            RouteMethod method;
            if (_storageProvider.TryGetSetting(RouteMethodProperty, out method))
                RouteMethod.Value = method;

            // Load travel mode.
            TravelMode mode;
            if (_storageProvider.TryGetSetting(RouteMethodProperty, out mode))
                TravelMode.Value = mode;

            // Load selected place.
            Guid selectedPlaceId;
            if (_storageProvider.TryGetSetting(SelectedPlaceProperty, out selectedPlaceId) && !_places.IsNullOrEmpty())
                SelectedPlace.Value = _places.FirstOrDefault(p => p.Id == selectedPlaceId);

            // Load selected route.
            Guid selectedRouteId;
            if (_storageProvider.TryGetSetting(SelectedPlaceProperty, out selectedRouteId) && !_routes.IsNullOrEmpty())
                SelectedRoute.Value = _routes.FirstOrDefault(r => r.Id == selectedRouteId);
        }

        /// <summary>
        ///   Toggles the specified map overlay.
        /// </summary>
        public void ToggleMapOverlay(Layer layer)
        {
            if (MapOverlays.Contains(layer))
                MapOverlays.Remove(layer);
            else
                MapOverlays.Add(layer);
        }

        /// <summary>
        ///   Removes the specified trip.
        /// </summary>
        /// <param name="trip"> The trip to be removed. </param>
        public void RemoveTrip(Trip trip)
        {
            _trips.Remove(trip);
        }

        /// <summary>
        ///   Removes all trips.
        /// </summary>
        public void ClearTrips()
        {
            _trips.Clear();
        }

        /// <summary>
        ///   Removes all personal places.
        /// </summary>
        public void ClearPersonalPlaces()
        {
            for (var i = _places.Count - 1; i >= 0; i--)
                if (!_places[i].IsSearchResult)
                    _places.RemoveAt(i);
        }

        #endregion


        #region Private Methods

        private void ProcessCall<TResponse, TResult>(Action<IGoogleMapsClient, Action<RestResponse<TResponse>>> callAction, Action<TResponse> processSuccessfulResponse = null, Action<CallbackEventArgs> callback = null)
            where TResponse : class, IResponse<TResult>
            where TResult : class
        {
            callAction(_googleMapsClient, r =>
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
                    (result is IList && ((IList)result).Count == 0))
                {
                    var exception = r.ErrorException;
                    if (exception != null)
                        callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.ServiceException, exception));
                    else if (data == null || data.Status != StatusCodes.ZERO_RESULTS)
                        callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.Unknown));
                    else
                        callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.EmptyResult));
                    return;
                }
                processSuccessfulResponse.ExecuteIfNotNull(r.Data);
                callback.ExecuteIfNotNull(new CallbackEventArgs());
            });
        }

        private void ProcessCall<TResponse, TResult, TCallback>(Action<IGoogleMapsClient, Action<RestResponse<TResponse>>> callAction, Func<TResponse, TCallback> processSuccessfulResponse = null, Action<CallbackEventArgs<TCallback>> callback = null)
            where TResponse : class, IResponse<TResult>
            where TResult : class
        {
            callAction(_googleMapsClient, r =>
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
                    (result is IList && ((IList)result).Count == 0))
                {
                    var exception = r.ErrorException;
                    if (exception != null)
                        callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.ServiceException, exception));
                    else if (data == null || data.Status != StatusCodes.ZERO_RESULTS)
                        callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.Unknown));
                    else
                        callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(CallbackStatus.EmptyResult));
                    return;
                }
                var callbackResult = processSuccessfulResponse.ExecuteIfNotNull(data);
                callback.ExecuteIfNotNull(new CallbackEventArgs<TCallback>(callbackResult));
            });
        }

        #endregion


        #region Constructors

        static DataContext()
        {
            _placeIconMap = new Dictionary<PlaceIcon, string>
            {
                {PlaceIcon.General, "general"},
                {PlaceIcon.Recreation, "recreation"},
                {PlaceIcon.Drink, "bar and pub"},
                {PlaceIcon.Fuel, "fuel and service station"},
                {PlaceIcon.Vehicle, "aotomotive"},
                {PlaceIcon.Shop, "shop"},
                {PlaceIcon.Property, "property and house"},
                {PlaceIcon.Restaurant, "restaurant"},
                {PlaceIcon.Airport, "airport"},
                {PlaceIcon.PublicTransport, "public transport"},
                {PlaceIcon.Information, "information"},
                {PlaceIcon.Internet, "internet"},
                {PlaceIcon.MoneyExchange, "money exchange"},
                {PlaceIcon.Ferry, "ferry"},
                {PlaceIcon.Casino, "casino"},
            };
            _placeIconMap = _placeIconMap.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            _elementColorMap = new Dictionary<ElementColor, string>();
            foreach (var field in typeof(ElementColor).GetFields(BindingFlags.Static | BindingFlags.Public))
                _elementColorMap.Add((ElementColor)field.GetValue(null), field.Name.ToLower());
            _elementColorMap = _elementColorMap.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DataContext" /> class.
        /// </summary>
        public DataContext(IStorage storageProvider, ISerializer<byte[]> binerySerializer, IGoogleMapsClient googleMapsClient)
        {
            _storageProvider = storageProvider;
            _binarySerializer = binerySerializer;
            _googleMapsClient = googleMapsClient;

            MapCenter = new ObservableValue<GeoCoordinate>();
            MapZoomLevel = new ObservableValue<double>(1D);
            MapBaseLayer = new ObservableValue<Layer>();
            SearchInput = new ObservableValue<string>();
            RouteMethod = new ObservableValue<RouteMethod>();
            TravelMode = new ObservableValue<TravelMode>();
            Unit = new ObservableValue<Units>();
            SelectedPlace = new ObservableValue<Place>();
            SelectedRoute = new ObservableValue<Route>();

            MapOverlays = new ObservableCollection<Layer>();
            Places = new ReadOnlyObservableCollection<Place>(_places = new ObservableCollection<Place>());
            Routes = new ReadOnlyObservableCollection<Route>(_routes = new ObservableCollection<Route>());
            Trips = new ReadOnlyObservableCollection<Trip>(_trips = new ObservableCollection<Trip>());
        }

        #endregion
    }
}