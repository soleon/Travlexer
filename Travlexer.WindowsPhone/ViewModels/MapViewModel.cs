﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codify;
using Codify.Collections;
using Codify.Commands;
using Codify.Entities;
using Codify.Extensions;
using Codify.GoogleMaps.Controls;
using Codify.Services;
using Codify.Threading;
using Codify.WindowsPhone;
using Travlexer.Data;
using Travlexer.WindowsPhone.Converters;
using Travlexer.WindowsPhone.Infrastructure;
using Travlexer.WindowsPhone.Views;

namespace Travlexer.WindowsPhone.ViewModels
{
    /// <summary>
    /// Defines the view model for <see cref="MapView"/>
    /// </summary>
    public class MapViewModel : NotifyableEntity
    {
        #region Constants

        private const string CurrentLocationString = "Current Location";

        #endregion


        #region Private Members

        private readonly GeoCoordinateWatcher _geoWatcher;

        private AppBarButtonViewModel
            _trackButton,
            _keepSearchResultButton,
            _deletePlaceButton;

        private ObservableCollection<AppBarButtonViewModel>
            _defaultButtonItemsSource,
            _routeSelectedButtonItemsSource,
            _pushpinSelectedButtonItemsSource;

        private ObservableCollection<AppBarMenuItemViewModel>
            _appBarMenuItemsSource;

        #endregion


        #region Public Enums

        public enum VisualStates : byte
        {
            Default = 0,
            Search,
            PushpinSelected,
            RouteSelected,
            Drag,
            Route
        }

        #endregion


        #region Public Events

        /// <summary>
        /// Occurs when the execution of <see cref="CommandSearch"/> is completed successfully.
        /// </summary>
        public event Action<IList<Place>> SearchSucceeded;

        /// <summary>
        /// Occurs when <see cref="Suggestions"/> are retrieved from service client.
        /// </summary>
        public event Action SuggestionsRetrieved;

        /// <summary>
        /// Occurs when <see cref="CommandRoute"/> is successfully finished.
        /// </summary>
        public event Action<Route> RouteSucceeded;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel"/> class.
        /// </summary>
        public MapViewModel()
        {
            // Initialise local properties.
            VisualState = new ObservableValue<VisualStates>();
            Suggestions = new ReadOnlyObservableCollection<SearchSuggestion>(_suggestions);
            Pushpins = new AdaptedObservableCollection<Place, PlaceViewModel>(p => new PlaceViewModel(p, this), ApplicationContext.Data.Places);
            Routes = new AdaptedObservableCollection<Route, RouteViewModel>(r => new RouteViewModel(r, this), ApplicationContext.Data.Routes);
            DepartureLocation = new RouteLocation();
            ArrivalLocation = new RouteLocation();

            // Initialise commands.
            CommandGetSuggestions = new DelegateCommand(OnGetSuggestions);
            CommandSearch = new DelegateCommand(OnSearch);
            CommandAddPlace = new DelegateCommand<Location>(OnAddPlace);
            CommandSelectPushpin = new DelegateCommand<PlaceViewModel>(vm => SelectedPushpin = vm);
            CommandSelectRoute = new DelegateCommand<RouteViewModel>(vm => SelectedRoute = vm);
            CommandDeletePlace = new DelegateCommand<PlaceViewModel>(OnDeletePlace);
            CommandUpdatePlace = new DelegateCommand<PlaceViewModel>(OnUpdatePlace);
            CommandStopTrackingCurrentLocation = new DelegateCommand(OnStopTrackingCurrentLocation);
            CommandGoToDefaultState = new DelegateCommand(OnGoToDefaultState);
            CommandStartGeoWatcher = new DelegateCommand(() => _geoWatcher.Start());
            CommandStopGeoWatcher = new DelegateCommand(() => _geoWatcher.Stop());
            CommandAddCurrentPlace = new DelegateCommand(() => OnAddPlace(CurrentLocation.ToLocalLocation()), () => CurrentLocation != null && !CurrentLocation.IsUnknown);
            CommandZoomIn = new DelegateCommand(() =>
            {
                if (ZoomLevel.Value < 20D)
                    ZoomLevel.Value = Math.Min(ZoomLevel.Value += 1D, 20D);
            });
            CommandZoomOut = new DelegateCommand(() =>
            {
                if (ZoomLevel.Value > 1D)
                    ZoomLevel.Value = Math.Max(ZoomLevel.Value -= 1D, 1D);
            });
            CommandShowStreetLayer = new DelegateCommand(() => ApplicationContext.Data.MapBaseLayer.Value = Layer.Street);
            CommandShowSatelliteHybridLayer = new DelegateCommand(() => ApplicationContext.Data.MapBaseLayer.Value = Layer.SatelliteHybrid);
            CommandToggleMapOverlay = new DelegateCommand<Layer>(ApplicationContext.Data.ToggleMapOverlay);
            CommandToggleToolbar = new DelegateCommand(ApplicationContext.Configuration.ToggleToolbarState);
            CommandSetDepartLocationToCurrentLocation = new DelegateCommand(() => DepartureLocation.Address = CurrentLocationString);
            CommandSetArriveLocationToCurrentLocation = new DelegateCommand(() => ArrivalLocation.Address = CurrentLocationString);
            CommandRoute = new DelegateCommand(OnRoute);
            CommandActivate = new DelegateCommand(OnActivate);
            CommandDeactivate = new DelegateCommand(OnDeactivate);

            // Initialise geo-coordinate watcher.
            _geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 10D };
            _geoWatcher.PositionChanged += OnGeoWatcherPositionChanged;


            // Initialise application bar button and menu items sources.
            InitializeAppBarButtonSources();
            InitializeAppBarMenuItemsSource();

            // Handle necessary events.
            Pushpins.CollectionChanged += OnPushpinsCollectionChanged;
            ApplicationContext.Data.MapBaseLayer.ValueChanged += (old, @new) => RaisePropertyChanged(IsStreetLayerVisibleProperty, IsSatelliteHybridLayerVisibleProperty);
            ApplicationContext.Data.MapOverlays.CollectionChanged += (s, e) => RaisePropertyChanged(IsTrafficLayerVisibleProperty, IsTransitLayerVisibleProperty);
            VisualState.ValueChanged += OnVisualStateChanged;
            IsTrackingCurrentLocation.ValueChanged += OnIsTrackingCurrentLocationValueChanged;

            // Automatically track current position at first run.
            if (ApplicationContext.Configuration.IsFirstRun)
            {
                IsTrackingCurrentLocation.Value = true;
            }

            // Try centering on current location if available.
            if (IsTrackingCurrentLocation.Value)
            {
                if (!_geoWatcher.Position.Location.IsUnknown)
                {
                    Center.Value = _geoWatcher.Position.Location;
                    ZoomLevel.Value = 15;
                }
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the collection of all user pins.
        /// </summary>
        public AdaptedObservableCollection<Place, PlaceViewModel> Pushpins { get; private set; }

        /// <summary>
        /// Gets or sets the selected place.
        /// </summary>
        public PlaceViewModel SelectedPushpin
        {
            get { return _selectedPushpin; }
            set
            {
                if (!SetValue(ref _selectedPushpin, value, SelectedPushpinProperty))
                {
                    return;
                }
                if (value == null)
                {
                    VisualState.Value = VisualStates.Default;
                }
                else
                {
                    DragPushpin = null;
                    var place = value.Data;
                    Center.Value = place.Location.ToGeoCoordinate();
                    if (place.DataState != DataStates.Finished)
                    {
                        OnUpdatePlace(value);
                    }
                    if (value.Data.IsSearchResult)
                    {
                        _pushpinSelectedButtonItemsSource[3] = _keepSearchResultButton;
                    }
                    else
                    {
                        _pushpinSelectedButtonItemsSource[3] = _deletePlaceButton;
                    }
                    VisualState.Value = VisualStates.PushpinSelected;
                }
                ApplicationContext.Data.SelectedPlace = value == null ? null : value.Data;
            }
        }

        private PlaceViewModel _selectedPushpin;
        private const string SelectedPushpinProperty = "SelectedPushpin";

        /// <summary>
        /// Gets all routes planned by the user.
        /// </summary>
        public AdaptedObservableCollection<Route, RouteViewModel> Routes { get; private set; }

        /// <summary>
        /// Gets or sets the selected route.
        /// </summary>
        public RouteViewModel SelectedRoute
        {
            get { return _selectedRoute; }
            set
            {
                if (!SetValue(ref _selectedRoute, value, SelectedRouteProperty))
                {
                    return;
                }
                VisualState.Value = value == null ? VisualStates.Default : VisualStates.RouteSelected;
            }
        }

        private RouteViewModel _selectedRoute;
        private const string SelectedRouteProperty = "SelectedRoute";

        /// <summary>
        /// Gets or sets the pushpin that's been dragged.
        /// </summary>
        public PlaceViewModel DragPushpin
        {
            get { return _dragPushpin; }
            set
            {
                if (!SetValue(ref _dragPushpin, value, DragPushpinProperty))
                {
                    return;
                }
                if (value == null)
                {
                    VisualState.Value = VisualStates.Default;
                }
                else
                {
                    SelectedPushpin = null;
                    VisualState.Value = VisualStates.Drag;
                    IsTrackingCurrentLocation.Value = false;
                }
            }
        }

        private PlaceViewModel _dragPushpin;
        private const string DragPushpinProperty = "DragPushpin";

        public GeoCoordinate CurrentLocation
        {
            get { return _currentLocation; }
            set { SetValue(ref _currentLocation, value, CurrentLocationProperty); }
        }

        private GeoCoordinate _currentLocation;
        private const string CurrentLocationProperty = "CurrentLocation";

        /// <summary>
        /// Gets or sets the map center geo-coordination.
        /// </summary>
        public ObservableValue<GeoCoordinate> Center
        {
            get { return ApplicationContext.Data.MapCenter; }
        }

        /// <summary>
        /// Gets or sets the zoom level of the map.
        /// </summary>
        public ObservableValue<double> ZoomLevel
        {
            get { return ApplicationContext.Data.MapZoomLevel; }
        }

        /// <summary>
        /// Gets the suggestions based on the <see cref="SearchInput"/>.
        /// </summary>
        public ReadOnlyObservableCollection<SearchSuggestion> Suggestions { get; private set; }

        private readonly ObservableCollection<SearchSuggestion> _suggestions = new ObservableCollection<SearchSuggestion>();

        /// <summary>
        /// Gets or sets the selected <see cref="Travlexer.Data.SearchSuggestion"/>.
        /// </summary>
        public SearchSuggestion SelectedSuggestion
        {
            get { return _selectedSuggestion; }
            set
            {
                if (SetValue(ref _selectedSuggestion, value, SelectedSuggestionProperty) && value != null)
                {
                    OnSuggestionSelected();
                }
            }
        }

        private SearchSuggestion _selectedSuggestion;
        private const string SelectedSuggestionProperty = "SelectedSuggestion";

        /// <summary>
        /// Gets or sets the search input.
        /// </summary>
        public ObservableValue<string> SearchInput
        {
            get { return ApplicationContext.Data.SearchInput; }
        }

        /// <summary>
        /// Gets the visual state of the view.
        /// </summary>
        public ObservableValue<VisualStates> VisualState { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the map is tracking current location.
        /// </summary>
        public ObservableValue<bool> IsTrackingCurrentLocation
        {
            get { return ApplicationContext.Configuration.IsTrackingCurrentLocation; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public ObservableValue<bool> IsBusy
        {
            get { return ApplicationContext.Configuration.IsBusy; }
        }

        /// <summary>
        /// Gets the visibility of online street layer.
        /// </summary>
        public Visibility IsStreetLayerVisible
        {
            get { return ApplicationContext.Data.MapBaseLayer.Value == Layer.Street ? Visibility.Visible : Visibility.Collapsed; }
        }

        private const string IsStreetLayerVisibleProperty = "IsStreetLayerVisible";

        /// <summary>
        /// Gets the visibility of online satellite hybrid layer.
        /// </summary>
        public Visibility IsSatelliteHybridLayerVisible
        {
            get { return ApplicationContext.Data.MapBaseLayer.Value == Layer.SatelliteHybrid ? Visibility.Visible : Visibility.Collapsed; }
        }

        private const string IsSatelliteHybridLayerVisibleProperty = "IsSatelliteHybridLayerVisible";

        /// <summary>
        /// Gets the visibility of online traffic layer.
        /// </summary>
        public Visibility IsTrafficLayerVisible
        {
            get { return ApplicationContext.Data.MapOverlays.Contains(Layer.TrafficOverlay) ? Visibility.Visible : Visibility.Collapsed; }
        }

        private const string IsTrafficLayerVisibleProperty = "IsTrafficLayerVisible";

        /// <summary>
        /// Gets the visibility of online transit layer.
        /// </summary>
        public Visibility IsTransitLayerVisible
        {
            get { return ApplicationContext.Data.MapOverlays.Contains(Layer.TransitOverlay) ? Visibility.Visible : Visibility.Collapsed; }
        }

        private const string IsTransitLayerVisibleProperty = "IsTransitLayerVisible";

        /// <summary>
        /// Gets the state of the toolbar.
        /// </summary>
        public ObservableValue<ExpansionStates> ToolbarState
        {
            get { return ApplicationContext.Configuration.ToolbarState; }
        }

        /// <summary>
        /// Gets the value indicates if the application is working in offline mode.
        /// </summary>
        public ObservableValue<bool> IsOnline
        {
            get { return ApplicationContext.Configuration.IsOnline; }
        }

        /// <summary>
        /// Gets the available route modes.
        /// </summary>
        public List<KeyValueIcon<TravelMode, string, ImageBrush>> TravelModes
        {
            get { return RouteModeKeyValueIconConverter.TravelModes; }
        }

        /// <summary>
        /// Gets the selected route mode.
        /// </summary>
        public ObservableValue<TravelMode> SelectedTravelMode
        {
            get { return ApplicationContext.Data.TravelMode; }
        }

        /// <summary>
        /// Gets the available route methods.
        /// </summary>
        public List<KeyValueIcon<RouteMethod, string, ImageBrush>> RouteMethods
        {
            get { return RouteMethodKeyValueIconConverter.RouteMethods; }
        }

        /// <summary>
        /// Gets the selected route method.
        /// </summary>
        public ObservableValue<RouteMethod> SelectedRouteMethod
        {
            get { return ApplicationContext.Data.RouteMethod; }
        }

        /// <summary>
        /// Gets the departure location.
        /// </summary>
        public RouteLocation DepartureLocation { get; private set; }

        /// <summary>
        /// Gets the arrival location.
        /// </summary>
        public RouteLocation ArrivalLocation { get; private set; }

        /// <summary>
        /// Gets the application bar button items sources.
        /// </summary>
        public ObservableCollection<AppBarButtonViewModel>[] AppBarButtonItemsSources { get; private set; }

        /// <summary>
        /// Gets the selected app bar button items source.
        /// </summary>
        public ObservableCollection<AppBarButtonViewModel> SelectedAppBarButtonItemsSource
        {
            get { return _selectedAppBarButtonItemsSource; }
            private set { SetValue(ref _selectedAppBarButtonItemsSource, value, SelectedAppBarButtonItemsSourceProperty); }
        }

        private ObservableCollection<AppBarButtonViewModel> _selectedAppBarButtonItemsSource;
        private const string SelectedAppBarButtonItemsSourceProperty = "SelectedAppBarButtonItemsSource";

        public ObservableCollection<AppBarMenuItemViewModel> SelectedAppBarMenuItemsSource
        {
            get { return _selectedAppBarMenuItemsSource; }
            private set { SetValue(ref _selectedAppBarMenuItemsSource, value, SelectedAppBarMenuItemsSourceProperty); }
        }

        private ObservableCollection<AppBarMenuItemViewModel> _selectedAppBarMenuItemsSource;
        private const string SelectedAppBarMenuItemsSourceProperty = "SelectedAppBarMenuItemsSource";

        /// <summary>
        /// Gets a value indicating whether this the application bar is visible.
        /// </summary>
        public bool IsAppBarVisible
        {
            get { return _isAppBarVisible; }
            private set { SetValue(ref _isAppBarVisible, value, IsAppBarVisibleProperty); }
        }

        private bool _isAppBarVisible = true;
        private const string IsAppBarVisibleProperty = "IsAppBarVisible";

        #endregion


        #region Commands

        /// <summary>
        /// Gets the command that adds a user pin.
        /// </summary>
        public DelegateCommand<Location> CommandAddPlace { get; private set; }

        /// <summary>
        /// Gets the command that toggles the pushpin content state.
        /// </summary>
        public DelegateCommand<PlaceViewModel> CommandSelectPushpin { get; private set; }

        /// <summary>
        /// Gets the command that toggles the pushpin content state.
        /// </summary>
        public DelegateCommand<RouteViewModel> CommandSelectRoute { get; private set; }

        /// <summary>
        /// Gets the command deletes a user pin.
        /// </summary>
        public DelegateCommand<PlaceViewModel> CommandDeletePlace { get; private set; }

        /// <summary>
        /// Gets the command that gets suggestions that based on the <see cref="SearchInput"/>.
        /// </summary>
        public DelegateCommand CommandGetSuggestions { get; private set; }

        /// <summary>
        /// Gets the command that performs the search based on the <see cref="SearchInput"/>.
        /// </summary>
        public DelegateCommand CommandSearch { get; private set; }

        /// <summary>
        /// Gets the command that stops tracking current location.
        /// </summary>
        public DelegateCommand CommandStopTrackingCurrentLocation { get; private set; }

        /// <summary>
        /// Gets the command that sets the <see cref="VisualState"/> to <see cref="VisualStates.Default"/>.
        /// </summary>
        public DelegateCommand CommandGoToDefaultState { get; private set; }

        /// <summary>
        /// Gets the command that updates the information of a given place.
        /// </summary>
        public DelegateCommand<PlaceViewModel> CommandUpdatePlace { get; private set; }

        /// <summary>
        /// Gets the command that starts geo coordinate watcher.
        /// </summary>
        public DelegateCommand CommandStartGeoWatcher { get; private set; }

        /// <summary>
        /// Gets the command that stops geo coordinate watcher.
        /// </summary>
        public DelegateCommand CommandStopGeoWatcher { get; private set; }

        /// <summary>
        /// Gets the command that adds a place at the <see cref="CurrentLocation"/>.
        /// </summary>
        public DelegateCommand CommandAddCurrentPlace { get; private set; }

        /// <summary>
        /// Gets the command that zooms in the map.
        /// </summary>
        public DelegateCommand CommandZoomIn { get; private set; }

        /// <summary>
        /// Gets the command that zooms out the map.
        /// </summary>
        public DelegateCommand CommandZoomOut { get; private set; }

        /// <summary>
        /// Gets the command that shows the street layer.
        /// </summary>
        public DelegateCommand CommandShowStreetLayer { get; private set; }

        /// <summary>
        /// Gets the command that shows the satellite hybrid layer.
        /// </summary>
        public DelegateCommand CommandShowSatelliteHybridLayer { get; private set; }

        /// <summary>
        /// Gets the command that toggles traffic layer.
        /// </summary>
        public DelegateCommand<Layer> CommandToggleMapOverlay { get; private set; }

        /// <summary>
        /// Gets the command that toggles the <see cref="ToolbarState"/>.
        /// </summary>
        public DelegateCommand CommandToggleToolbar { get; private set; }

        /// <summary>
        /// Gets the command that sets depart location to current location.
        /// </summary>
        public DelegateCommand CommandSetDepartLocationToCurrentLocation { get; private set; }

        /// <summary>
        /// Gets the command that sets arrive location to current location.
        /// </summary>
        public DelegateCommand CommandSetArriveLocationToCurrentLocation { get; private set; }

        /// <summary>
        /// Gets the command that finds a route.
        /// </summary>
        public DelegateCommand CommandRoute { get; private set; }

        public DelegateCommand CommandActivate { get; private set; }

        public DelegateCommand CommandDeactivate { get; private set; }

        #endregion


        #region Event Handling

        /// <summary>
        /// Called when <see cref="CommandGoToDefaultState"/>.
        /// </summary>
        private void OnGoToDefaultState()
        {
            ApplicationContext.Data.CancelGetSuggestions();
            VisualState.Value = VisualStates.Default;
        }

        /// <summary>
        /// Called when <see cref="CommandAddPlace"/> is executed.
        /// </summary>
        private void OnAddPlace(Location location)
        {
            VisualState.Value = VisualStates.Default;
            ApplicationContext.Data.AddNewPlace(location);
        }

        /// <summary>
        /// Called when <see cref="CommandDeletePlace"/> is executed.
        /// </summary>
        private void OnDeletePlace(PlaceViewModel vm)
        {
            ApplicationContext.Data.RemovePlace(vm.Data);
        }

        /// <summary>
        /// Called when <see cref="CommandDeletePlace"/> is executed.
        /// </summary>
        private void OnDeleteSelectedPlace()
        {
            ApplicationContext.Data.RemovePlace(SelectedPushpin.Data);
            SelectedPushpin = null;
        }

        /// <summary>
        /// Called when the "keep" application bar button is pressed when a search result is selected.
        /// </summary>
        private void OnKeepSelectedSearchResult()
        {
            SelectedPushpin.Data.IsSearchResult = false;
            _pushpinSelectedButtonItemsSource[3] = _deletePlaceButton;
        }

        /// <summary>
        /// Called when items in <see cref="Pushpins"/> collection are changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnPushpinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (var pushpin in e.OldItems)
                    {
                        if (SelectedPushpin == pushpin)
                        {
                            SelectedPushpin = null;
                        }
                        return;
                    }
                    break;
            }
        }

        /// <summary>
        /// Called when <see cref="CommandSearch"/> is executed.
        /// </summary>
        private void OnSearch()
        {
            if (SearchInput.Value.IsNullOrEmpty()) return;
            ApplicationContext.Configuration.IsBusy.Value = true;
            VisualState.Value = VisualStates.Default;
            ApplicationContext.Data.CancelGetSuggestions();
            ApplicationContext.Data.Search(Center.Value.ToLocalLocation(), SearchInput.Value, callback =>
            {
                ApplicationContext.Configuration.IsBusy.Value = false;
                if (callback.Status != CallbackStatus.Successful)
                {
                    MessageBox.Show("Sorry, we couldn't find anything for you.", "Nothing Was Found", MessageBoxButton.OK);
                    return;
                }
                IsTrackingCurrentLocation.Value = false;
                SearchSucceeded.ExecuteIfNotNull(callback.Result);
            });
            ResetSearchSuggestions();
        }

        /// <summary>
        /// Called when <see cref="CommandGetSuggestions"/> is executed.
        /// </summary>
        private void OnGetSuggestions()
        {
            // Do not get suggestion visual state is not Search.
            if (VisualState.Value != VisualStates.Search)
            {
                return;
            }

            ApplicationContext.Configuration.IsBusy.Value = true;
            ApplicationContext.Data.GetSuggestions(Center.Value.ToLocalLocation(), SearchInput.Value, args =>
            {
                ApplicationContext.Configuration.IsBusy.Value = false;
                SelectedSuggestion = null;
                _suggestions.Clear();
                if (args.Status != CallbackStatus.Successful)
                {
                    return;
                }
                var suggestions = args.Result;
                suggestions.ForEach(_suggestions.Add);
                SuggestionsRetrieved.ExecuteIfNotNull();
            });
        }

        /// <summary>
        /// Called when <see cref="SelectedSuggestion"/> is changed to another valid value.
        /// </summary>
        private void OnSuggestionSelected()
        {
            // Invoke the state change async to hack a problem that the phone keyboard doesn't retract even when the focus is not on the search text box.
            UIThread.InvokeBack(() => VisualState.Value = VisualStates.Default);

            ApplicationContext.Configuration.IsBusy.Value = true;
            ApplicationContext.Data.GetPlaceDetails(SelectedSuggestion.Reference, args =>
            {
                ApplicationContext.Configuration.IsBusy.Value = false;
                if (args.Status != CallbackStatus.Successful)
                {
                    MessageBox.Show("Sorry, we couldn't get any information for your selected place.", "No Information Found", MessageBoxButton.OK);
                    return;
                }
                IsTrackingCurrentLocation.Value = false;
                SearchSucceeded.ExecuteIfNotNull(new List<Place> { args.Result });
            });

            ResetSearchSuggestions();
        }

        /// <summary>
        /// Called when <see cref="CommandStopTrackingCurrentLocation"/> is executed.
        /// </summary>
        private void OnStopTrackingCurrentLocation()
        {
            IsTrackingCurrentLocation.Value = false;
        }

        /// <summary>
        /// Called when <see cref="CommandStopTrackingCurrentLocation"/> is executed.
        /// </summary>
        private void OnStartTrackingCurrentLocation()
        {
            IsTrackingCurrentLocation.Value = true;
        }

        /// <summary>
        /// Called when <see cref="GeoCoordinateWatcher.PositionChanged"/> event is raised.
        /// </summary>
        private void OnGeoWatcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var location = e.Position.Location;
            if (location.IsUnknown)
            {
                return;
            }
            var oldCurrentLocation = CurrentLocation;
            CurrentLocation = location;
            if (!IsTrackingCurrentLocation.Value)
            {
                return;
            }
            Center.Value = location;

            // This is to determine whether this is the first current location update.
            // The first update should set the zoom level to 15.
            if (oldCurrentLocation == null)
            {
                ZoomLevel.Value = 15;
            }
        }

        /// <summary>
        /// Called when <see cref="CommandUpdatePlace"/> is executed.
        /// </summary>
        /// <param name="pushpin">The pushpin view model.</param>
        private void OnUpdatePlace(PlaceViewModel pushpin)
        {
            ApplicationContext.Data.GetPlaceDetails(pushpin.Data);
        }

        /// <summary>
        /// Called when the value of <see cref="VisualState"/> is chagned.
        /// </summary>
        private void OnVisualStateChanged(VisualStates old, VisualStates @new)
        {
            switch (old)
            {
                case VisualStates.PushpinSelected:
                    SelectedPushpin = null;
                    break;
                case VisualStates.RouteSelected:
                    SelectedRoute = null;
                    break;
            }
            switch (@new)
            {
                case VisualStates.Default:
                    IsAppBarVisible = true;
                    SelectedAppBarButtonItemsSource = _defaultButtonItemsSource;
                    SelectedAppBarMenuItemsSource = _appBarMenuItemsSource;
                    break;
                case VisualStates.PushpinSelected:
                    IsAppBarVisible = true;
                    SelectedAppBarButtonItemsSource = _pushpinSelectedButtonItemsSource;
                    SelectedAppBarMenuItemsSource = null;
                    break;
                case VisualStates.RouteSelected:
                    IsAppBarVisible = true;
                    SelectedAppBarButtonItemsSource = _routeSelectedButtonItemsSource;
                    SelectedAppBarMenuItemsSource = null;
                    break;
                case VisualStates.Route:
                    IsAppBarVisible = false;
                    break;
                case VisualStates.Drag:
                    IsAppBarVisible = false;
                    break;
                case VisualStates.Search:
                    IsAppBarVisible = false;
                    break;
            }
        }

        /// <summary>
        /// Called when the value of <see cref="IsTrackingCurrentLocation"/> has changed.
        /// </summary>
        private void OnIsTrackingCurrentLocationValueChanged(bool old, bool @new)
        {
            _trackButton.IsEnabled = !@new;
            if (!@new || CurrentLocation == null || CurrentLocation.IsUnknown)
            {
                return;
            }
            Center.Value = CurrentLocation;
            ZoomLevel.Value = 15;
        }

        /// <summary>
        /// Called when <see cref="CommandRoute"/> is executed.
        /// </summary>
        private void OnRoute()
        {
            if (DepartureLocation.Address.IsNullOrEmpty() || ArrivalLocation.Address.IsNullOrEmpty())
            {
                return;
            }

            var departureLocation = DepartureLocation;
            var arrivalLocation = ArrivalLocation;

            string departureAddress;
            if (departureLocation.PlaceId != Guid.Empty)
            {
                var departurePlace = ApplicationContext.Data.Places.FirstOrDefault(p => p.Id == departureLocation.PlaceId);
                departureAddress = departurePlace == null ? departureLocation.Address : departurePlace.Location.ToString();
            }
            else
            {
                departureAddress = departureLocation.Address;
            }

            string arrivalAddress;
            if (arrivalLocation.PlaceId != Guid.Empty)
            {
                var arrivalPlace = ApplicationContext.Data.Places.FirstOrDefault(p => p.Id == arrivalLocation.PlaceId);
                arrivalAddress = arrivalPlace == null ? arrivalLocation.Address : arrivalPlace.Location.ToString();
            }
            else
            {
                arrivalAddress = arrivalLocation.Address;
            }

            if (departureAddress == arrivalAddress)
            {
                MessageBox.Show("We don't think finding routes between the same location is necessary.", "Same Location", MessageBoxButton.OK);
                return;
            }
            if (departureAddress == CurrentLocationString || arrivalAddress == CurrentLocationString)
            {
                if (CurrentLocation == null || CurrentLocation.IsUnknown)
                {
                    MessageBox.Show("Your current location is unavailable at the moment.", "No Current Location", MessageBoxButton.OK);
                    return;
                }
                if (departureAddress == CurrentLocationString)
                {
                    departureAddress = CurrentLocation.ToString();
                }
                else
                {
                    arrivalAddress = CurrentLocation.ToString();
                }
            }

            IsBusy.Value = true;
            VisualState.Value = VisualStates.Default;
            ApplicationContext.Data.GetRoute(departureAddress, arrivalAddress, SelectedTravelMode.Value, SelectedRouteMethod.Value, callback =>
            {
                IsBusy.Value = false;
                Route route;
                Collection<Location> points;
                int count;
                if (callback.Status != CallbackStatus.Successful || (count = (points = (route = callback.Result).Points).Count) <= 0)
                {
                    MessageBox.Show("Sorry, we couldn't find a route between the specified locations.", "No Routes Found", MessageBoxButton.OK);
                    return;
                }
                if (count <= 1)
                {
                    MessageBox.Show("It looks like the specified locations are too close to plan a route.", "Locations Too Close", MessageBoxButton.OK);
                    return;
                }

                IsTrackingCurrentLocation.Value = false;

                // Check if there's already a departure place.
                var place = ApplicationContext.Data.Places.FirstOrDefault(p => p.Id == departureLocation.PlaceId);
                if (place == null)
                {
                    var point = points[0];
                    place = ApplicationContext.Data.Places.FirstOrDefault(p => p.Location == point) ?? ApplicationContext.Data.AddNewPlace(point);
                    route.DeparturePlaceId = place.Id;
                }
                else
                {
                    route.DeparturePlaceId = departureLocation.PlaceId;
                }

                // Check if there's already an arrival place.
                place = ApplicationContext.Data.Places.FirstOrDefault(p => p.Id == arrivalLocation.PlaceId);
                if (place == null)
                {
                    var point = points[count - 1];
                    place = ApplicationContext.Data.Places.FirstOrDefault(p => p.Location == point) ?? ApplicationContext.Data.AddNewPlace(point);
                    route.ArrivalPlaceId = place.Id;
                }
                else
                {
                    route.ArrivalPlaceId = arrivalLocation.PlaceId;
                }

                RouteSucceeded.ExecuteIfNotNull(route);
            });
        }

        /// <summary>
        /// Called when "clear routes" menu item is pressed in the application bar.
        /// </summary>
        private void OnClearRoutes()
        {
            if (ApplicationContext.Data.Routes.Count == 0)
            {
                return;
            }
            if (MessageBox.Show("This will clear all routes on the map. Do you want to continue?", "Clear Routes", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ApplicationContext.Data.ClearRoutes();
            }
        }

        /// <summary>
        /// Called when the "arrive" application bar button is pressed when a pushpin is selected.
        /// </summary>
        private void OnSetSelectedPlaceAsArriveLocation()
        {
            var place = SelectedPushpin.Data;
            SelectedPushpin = null;
            var location = ArrivalLocation;
            location.Address = place.Address ?? place.Location.ToString();
            location.PlaceId = place.Id;
            VisualState.Value = VisualStates.Route;
        }

        /// <summary>
        /// Called when the "depart" application bar button is pressed when a pushpin is selected.
        /// </summary>
        private void OnSetSelectedPlaceAsDepartLocation()
        {
            var place = SelectedPushpin.Data;
            SelectedPushpin = null;
            var location = DepartureLocation;
            location.Address = place.Address ?? place.Location.ToString();
            location.PlaceId = place.Id;
            VisualState.Value = VisualStates.Route;
        }

        /// <summary>
        /// Called when the "remove" application bar button is pressed when a route is selected.
        /// </summary>
        private void OnDeleteSelectedRoute()
        {
            var route = SelectedRoute.Data;
            var result = MessageBox.Show("Do you want to remove the departure and arrival pins too?", "Also Remove Pins", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var count = 0;
                var places = ApplicationContext.Data.Places;
                for (var i = places.Count - 1; i >= 0; i--)
                {
                    var place = places[i];
                    var id = place.Id;
                    if (id != route.DeparturePlaceId && id != route.ArrivalPlaceId)
                    {
                        continue;
                    }
                    ApplicationContext.Data.RemovePlace(place);
                    if (count == 1)
                    {
                        break;
                    }
                    count++;
                }
            }
            ApplicationContext.Data.RemoveRoute(route);
            SelectedRoute = null;
        }

        /// <summary>
        /// Called when <see cref="Codify.WindowsPhone.NavigationService.BackKeyPress"/> event is fired.
        /// </summary>
        private void OnBackKeyPress(object sender, CancelEventArgs e)
        {
            if (VisualState.Value == VisualStates.Default)
            {
                return;
            }
            e.Cancel = true;
            VisualState.Value = VisualStates.Default;
        }

        /// <summary>
        /// Called when the "details" application bar button is pressed when a pushpin is selected.
        /// </summary>
        private void OnViewSelectedPlaceDetails()
        {
            ApplicationContext.NavigationService.Navigate<PlaceDetailsViewModel>();
        }

        /// <summary>
        /// Called when <see cref="CommandDeactivate"/> is executed.
        /// </summary>
        private void OnDeactivate()
        {
            ApplicationContext.NavigationService.BackKeyPress -= OnBackKeyPress;
        }

        /// <summary>
        /// Called when <see cref="CommandActivate"/> is executed.
        /// </summary>
        private void OnActivate()
        {
            ApplicationContext.NavigationService.BackKeyPress += OnBackKeyPress;
        }

        #endregion


        #region Private Methods

        private void ResetSearchSuggestions()
        {
            SelectedSuggestion = null;
            _suggestions.Clear();
        }

        private void InitializeAppBarButtonSources()
        {
            _keepSearchResultButton = new AppBarButtonViewModel
            {
                IconUri = new Uri("/Assets/Pin.png", UriKind.Relative),
                Text = "keep",
                Command = new DelegateCommand(OnKeepSelectedSearchResult)
            };
            AppBarButtonItemsSources = new[]
			{
				_defaultButtonItemsSource = new ObservableCollection<AppBarButtonViewModel>
				{
					(_trackButton = new AppBarButtonViewModel
                    {
                        IconUri = new Uri("/Assets/CurrentLocation.png", UriKind.Relative),
                        Text = "track",
                        IsEnabled = !IsTrackingCurrentLocation.Value,
                        Command = new DelegateCommand(OnStartTrackingCurrentLocation)
                    }),
					new AppBarButtonViewModel
					{
						IconUri = new Uri("/Assets/Search.png", UriKind.Relative),
						Text = "search",
						Command = new DelegateCommand(() => VisualState.Value = VisualStates.Search)
					},
					new AppBarButtonViewModel
					{
						IconUri = new Uri("/Assets/Route.png", UriKind.Relative),
						Text = "route",
						Command = new DelegateCommand(() => VisualState.Value = VisualStates.Route)
					},
                    new AppBarButtonViewModel
                    {
                        IconUri = new Uri("/Assets/CheckList.png", UriKind.Relative),
                        Text = "browse",
                        Command = new DelegateCommand(()=> ApplicationContext.NavigationService.Navigate<ManageViewModel>())
                    }
				},
				_routeSelectedButtonItemsSource = new ObservableCollection<AppBarButtonViewModel>
				{
					new AppBarButtonViewModel
					{
						IconUri = new Uri("/Assets/Information.png", UriKind.Relative),
						Text = "details"
					},
					new AppBarButtonViewModel
					{
						IconUri = new Uri("/Assets/Delete.png", UriKind.Relative),
						Text = "remove",
						Command = new DelegateCommand(OnDeleteSelectedRoute)
					}
				},
				_pushpinSelectedButtonItemsSource = new ObservableCollection<AppBarButtonViewModel>
				{
					new AppBarButtonViewModel
					{
						IconUri = new Uri("/Assets/Information.png", UriKind.Relative),
						Text = "details",
						Command = new DelegateCommand(OnViewSelectedPlaceDetails)
					},
					new AppBarButtonViewModel
					{
						IconUri = new Uri("/Assets/Depart.png", UriKind.Relative),
						Text = "depart",
						Command = new DelegateCommand(OnSetSelectedPlaceAsDepartLocation)
					},
					new AppBarButtonViewModel
					{
						IconUri = new Uri("/Assets/Arrive.png", UriKind.Relative),
						Text = "arrive",
						Command = new DelegateCommand(OnSetSelectedPlaceAsArriveLocation)
					},
					(_deletePlaceButton = new AppBarButtonViewModel
                    {
                        IconUri = new Uri("/Assets/Delete.png", UriKind.Relative),
                        Text = "delete",
                        Command = new DelegateCommand(OnDeleteSelectedPlace)
                    })
				}
			};
            SelectedAppBarButtonItemsSource = AppBarButtonItemsSources[0];
        }

        private void InitializeAppBarMenuItemsSource()
        {
            SelectedAppBarMenuItemsSource = _appBarMenuItemsSource = new ObservableCollection<AppBarMenuItemViewModel>
			{
				new AppBarMenuItemViewModel
				{
					Text = "clear search",
					Command = new DelegateCommand(ApplicationContext.Data.ClearSearchResults)
				},
				new AppBarMenuItemViewModel
				{
					Text = "clear routes",
					Command = new DelegateCommand(OnClearRoutes)
				}
			};
        }

        #endregion
    }
}
