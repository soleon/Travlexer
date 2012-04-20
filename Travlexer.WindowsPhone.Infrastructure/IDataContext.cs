using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using Codify.Entities;
using Codify.GoogleMaps.Controls;
using Codify.Services;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.Infrastructure
{
    public interface IDataContext
    {

        /// <summary>
        /// Gets the collection that contains all user pins.
        /// </summary>
        ReadOnlyObservableCollection<Place> Places { get; }

        /// <summary>
        /// Gets or sets the selected place.
        /// </summary>
        Place SelectedPlace { get; set; }

        /// <summary>
        /// Gets or sets the map center geo-location.
        /// </summary>
        ObservableValue<GeoCoordinate> MapCenter { get; }

        /// <summary>
        /// Gets or sets the map zoom level.
        /// </summary>
        ObservableValue<double> MapZoomLevel { get; }

        /// <summary>
        /// Gets or sets the search input.
        /// </summary>
        ObservableValue<string> SearchInput { get; }

        /// <summary>
        /// Gets the route method.
        /// </summary>
        ObservableValue<RouteMethod> RouteMethod { get; }

        /// <summary>
        /// Gets the route mode.
        /// </summary>
        ObservableValue<TravelMode> TravelMode { get; }

        /// <summary>
        /// Gets or sets the map base layer.
        /// </summary>
        ObservableValue<Layer> MapBaseLayer { get; }

        /// <summary>
        /// Gets the map overlays.
        /// </summary>
        ObservableCollection<Layer> MapOverlays { get; }

        /// <summary>
        /// Gets the collection of all routes planned by the user.
        /// </summary>
        ReadOnlyObservableCollection<Route> Routes { get; }

        /// <summary>
        /// Gets the unit system that is currently in use.
        /// </summary>
        ObservableValue<Units> Unit { get; }

        /// <summary>
        /// Gets a list of available place icons.
        /// </summary>
        Dictionary<PlaceIcon, string> PlaceIconMap { get; }

        /// <summary>
        /// Gets a list of available element colors.
        /// </summary>
        Dictionary<ElementColor, string> ElementColorMap { get; }

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
        /// Removes the specified route.
        /// </summary>
        void RemoveRoute(Route route);

        /// <summary>
        /// Removes all places.
        /// </summary>
        void ClearPlaces();

        /// <summary>
        /// Removes all search results.
        /// </summary>
        void ClearSearchResults();

        /// <summary>
        /// Gets information of the specified <see cref="Travlexer.Data.Place"/>.
        /// </summary>
        /// <param name="place">The place to get the information for.</param>
        /// <param name="callback">The callback to be executed after this process is finished.</param>
        void GetPlaceInformation(Place place, Action<CallbackEventArgs> callback = null);

        /// <summary>
        /// Gets information of the specified <see cref="Travlexer.Data.Place"/>.
        /// </summary>
        /// <param name="location">The geo-location to get the information for.</param>
        /// <param name="callback">The callback to be executed after this process is finished.</param>
        void GetAddress(Location location, Action<CallbackEventArgs<string>> callback = null);

        /// <summary>
        /// Gets the details for the specified place by its reference key.
        /// </summary>
        /// <param name="place">The place to get the details for.</param>
        /// <param name="callback">The callback to be executed after the process is finished.</param>
        void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null);

        /// <summary>
        /// Gets place details by its reference key.
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

        /// <summary>
        /// Cancels the current get suggestions operation if there is any.
        /// </summary>
        void CancelGetSuggestions();

        /// <summary>
        /// Finds the route between the depart location and arrive location.
        /// </summary>
        /// <param name="departure">The depart location.</param>
        /// <param name="arrival">The arrive location.</param>
        /// <param name="mode">The travel mode for the route.</param>
        /// <param name="method">The routing method for the route.</param>
        /// <param name="callback">The callback to execute after the process is finished.</param>
        void GetRoute(string departure, string arrival, TravelMode mode, RouteMethod method, Action<CallbackEventArgs<Route>> callback = null);

        /// <summary>
        /// Clears all routes.
        /// </summary>
        void ClearRoutes();

        /// <summary>
        /// Saves the data context to the storage provided by <see cref="DataContext.StorageProvider"/>.
        /// </summary>
        void SaveContext();

        /// <summary>
        /// Loads the data context from the storage provided by <see cref="DataContext.StorageProvider"/>.
        /// </summary>
        void LoadContext();

        /// <summary>
        /// Toggles the specified map overlay.
        /// </summary>
        void ToggleMapOverlay(Layer layer);
    }
}