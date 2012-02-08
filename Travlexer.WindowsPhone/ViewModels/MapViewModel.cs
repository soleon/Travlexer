using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Device.Location;
using System.Windows;
using Codify;
using Codify.Collections;
using Codify.Commands;
using Codify.Controls.Maps;
using Codify.Extensions;
using Codify.Models;
using Codify.Services;
using Codify.Threading;
using Codify.ViewModels;
using Travlexer.WindowsPhone.Infrastructure;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.Views;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the view model for <see cref="MapView"/>
	/// </summary>
	public class MapViewModel : ViewModelBase
	{
		#region Private Members

		private readonly GeoCoordinateWatcher _geoWatcher;

		#endregion


		#region Public Enums

		public enum VisualStates : byte
		{
			Default = 0,
			Search,
			PushpinSelected,
			Drag
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

		#endregion


		#region Constructors

		public MapViewModel()
		{
			// Get values from global context.
			Center = DataContext.MapCenter;
			ZoomLevel = DataContext.MapZoomLevel;
			SearchInput = DataContext.SearchInput;
			IsTrackingCurrentLocation = DataContext.IsTrackingCurrentLocation;
			ToolbarState = ApplicationContext.ToolbarState;
			VisualState = new ObservableValue<VisualStates>();

			// Initialise collections.
			Suggestions = new ReadOnlyObservableCollection<SearchSuggestion>(_suggestions);
			Pushpins = new AdaptedObservableCollection<Place, DataViewModel<Place>>(p => new DataViewModel<Place>(p, this), DataContext.Places);
			Pushpins.CollectionChanged += OnPushpinsCollectionChanged;

			// Initialise commands.
			CommandGetSuggestions = new DelegateCommand(OnGetSuggestions);
			CommandSearch = new DelegateCommand(OnSearch);
			CommandAddPlace = new DelegateCommand<Location>(OnAddPlace);
			CommandSelectPushpin = new DelegateCommand<DataViewModel<Place>>(OnSelectPushpin);
			CommandDeselectPushpin = new DelegateCommand<DataViewModel<Place>>(OnDeselectPushpin);
			CommandDeletePlace = new DelegateCommand<DataViewModel<Place>>(OnDeletePlace);
			CommandPinSearchResult = new DelegateCommand<DataViewModel<Place>>(OnPinSearchResult);
			CommandUpdatePlace = new DelegateCommand<DataViewModel<Place>>(OnUpdatePlace);
			CommandStartTrackingCurrentLocation = new DelegateCommand(OnStartTrackingCurrentLocation);
			CommandStopTrackingCurrentLocation = new DelegateCommand(OnStopTrackingCurrentLocation);
			CommandGoToSearchState = new DelegateCommand(OnGoToSearchState);
			CommandGoToDefaultState = new DelegateCommand(OnGoToDefaultState);
			CommandClearSearchResults = new DelegateCommand(OnClearSearchResults);
			CommandStartGeoWatcher = new DelegateCommand(OnStartGeoWatcher);
			CommandStopGeoWatcher = new DelegateCommand(OnStopGeoWatcher);
			CommandAddCurrentPlace = new DelegateCommand(() => OnAddPlace(CurrentLocation), () => CurrentLocation != null && !CurrentLocation.IsUnknown);
			CommandZoomIn = new DelegateCommand(() => ZoomLevel++);
			CommandZoomOut = new DelegateCommand(() => ZoomLevel--);
			CommandShowStreetLayer = new DelegateCommand(() => DataContext.MapBaseLayer.Value = GoogleMapsLayer.Street);
			CommandShowSatelliteHybridLayer = new DelegateCommand(() => DataContext.MapBaseLayer.Value = GoogleMapsLayer.SatelliteHybrid);
			CommandToggleMapOverlay = new DelegateCommand<GoogleMapsLayer>(DataContext.ToggleMapOverlay);
			CommandToggleToolbar = new DelegateCommand(ApplicationContext.ToggleToolbarState);

			// Initialise geo-coordinate watcher.
			_geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 10D };
			_geoWatcher.PositionChanged += OnGeoWatcherPositionChanged;

			// Handle necessary events.
			DataContext.MapBaseLayer.ValueChanged += (old, @new) => RaisePropertyChange(IsStreetLayerVisibleProperty, IsSatelliteHybridLayerVisibleProperty);
			DataContext.MapOverlays.CollectionChanged += (s, e) => RaisePropertyChange(IsTrafficLayerVisibleProperty, IsTransitLayerVisibleProperty);
			ApplicationContext.IsBusy.ValueChanged += (oldValue, newValue) => RaisePropertyChange(IsBusyProperty);
			VisualState.ValueChanged += OnVisualStateChanged;

			// Automatically track current position at first run.
			if (DataContext.IsFirstRun)
			{
				//IsTrackingCurrentLocation = true;
			}

			// Try centering on current location if available.
			if (IsTrackingCurrentLocation)
			{
				if (!_geoWatcher.Position.Location.IsUnknown)
				{
					Center = _geoWatcher.Position.Location;
					ZoomLevel = 15;
				}
			}
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection of all user pins.
		/// </summary>
		public AdaptedObservableCollection<Place, DataViewModel<Place>> Pushpins { get; private set; }

		/// <summary>
		/// Gets or sets the selected place.
		/// </summary>
		public DataViewModel<Place> SelectedPushpin
		{
			get { return _selectedPushpin; }
			set
			{
				if (!SetProperty(ref _selectedPushpin, value, SelectedPushpinProperty))
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
					Center = place.Location;
					if (place.DataState != DataStates.Finished)
					{
						OnUpdatePlace(value);
					}
					VisualState.Value = VisualStates.PushpinSelected;
				}
			}
		}

		private DataViewModel<Place> _selectedPushpin;
		private const string SelectedPushpinProperty = "SelectedPushpin";

		/// <summary>
		/// Gets or sets the pushpin that's been dragged.
		/// </summary>
		public DataViewModel<Place> DragPushpin
		{
			get { return _dragPushpin; }
			set
			{
				if (!SetProperty(ref _dragPushpin, value, DragPushpinProperty))
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
					IsTrackingCurrentLocation = false;
				}
			}
		}

		private DataViewModel<Place> _dragPushpin;
		private const string DragPushpinProperty = "DragPushpin";

		public GeoCoordinate CurrentLocation
		{
			get { return _currentLocation; }
			set { SetProperty(ref _currentLocation, value, CurrentLocationProperty); }
		}

		private GeoCoordinate _currentLocation;
		private const string CurrentLocationProperty = "CurrentLocation";

		/// <summary>
		/// Gets or sets the map center geo-coordination.
		/// </summary>
		public GeoCoordinate Center
		{
			get { return DataContext.MapCenter; }
			set
			{
				var mapCenter = DataContext.MapCenter;
				if (value == mapCenter)
				{
					return;
				}
				mapCenter.Latitude = value.Latitude;
				mapCenter.Longitude = value.Longitude;
				RaisePropertyChange(CenterProperty);
			}
		}

		private const string CenterProperty = "Center";

		/// <summary>
		/// Gets or sets the zoom level of the map.
		/// </summary>
		public double ZoomLevel
		{
			get { return DataContext.MapZoomLevel; }
			set
			{
				if (value < 1 || value > 20 || DataContext.MapZoomLevel.Equals(value))
				{
					return;
				}
				DataContext.MapZoomLevel = value;
				RaisePropertyChange(ZoomLevelProperty);
			}
		}

		private const string ZoomLevelProperty = "ZoomLevel";

		/// <summary>
		/// Gets the suggestions based on the <see cref="SearchInput"/>.
		/// </summary>
		public ReadOnlyObservableCollection<SearchSuggestion> Suggestions { get; private set; }

		private readonly ObservableCollection<SearchSuggestion> _suggestions = new ObservableCollection<SearchSuggestion>();

		/// <summary>
		/// Gets or sets the selected <see cref="SearchSuggestion"/>.
		/// </summary>
		public SearchSuggestion SelectedSuggestion
		{
			get { return _selectedSuggestion; }
			set
			{
				if (SetProperty(ref _selectedSuggestion, value, SelectedSuggestionProperty) && value != null)
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
		public string SearchInput
		{
			get { return DataContext.SearchInput; }
			set
			{
				if (DataContext.SearchInput == value)
				{
					return;
				}
				DataContext.SearchInput = value;
				RaisePropertyChange(SearchInputProperty);
			}
		}

		private const string SearchInputProperty = "SearchInput";

		/// <summary>
		/// Gets the visual state of the view.
		/// </summary>
		public ObservableValue<VisualStates> VisualState { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the map is tracking current location.
		/// </summary>
		public bool IsTrackingCurrentLocation
		{
			get { return DataContext.IsTrackingCurrentLocation; }
			set
			{
				if (DataContext.IsTrackingCurrentLocation == value)
				{
					return;
				}
				DataContext.IsTrackingCurrentLocation = value;
				if (value && CurrentLocation != null && !CurrentLocation.IsUnknown)
				{
					Center = CurrentLocation;
					ZoomLevel = 15;
				}
				RaisePropertyChange(IsTrackingCurrentLocationProperty);
			}
		}

		private const string IsTrackingCurrentLocationProperty = "IsTrackingCurrentLocation";

		/// <summary>
		/// Gets or sets a value indicating whether this instance is busy.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
		/// </value>
		public bool IsBusy
		{
			get { return ApplicationContext.IsBusy.Value; }
		}

		private const string IsBusyProperty = "IsBusy";

		/// <summary>
		/// Gets the visibility of street layer.
		/// </summary>
		public Visibility IsStreetLayerVisible
		{
			get { return DataContext.MapBaseLayer.Value == GoogleMapsLayer.Street ? Visibility.Visible : Visibility.Collapsed; }
		}

		private const string IsStreetLayerVisibleProperty = "IsStreetLayerVisible";

		/// <summary>
		/// Gets the visibility of satellite hybrid layer.
		/// </summary>
		public Visibility IsSatelliteHybridLayerVisible
		{
			get { return DataContext.MapBaseLayer.Value == GoogleMapsLayer.SatelliteHybrid ? Visibility.Visible : Visibility.Collapsed; }
		}

		private const string IsSatelliteHybridLayerVisibleProperty = "IsSatelliteHybridLayerVisible";

		/// <summary>
		/// Gets the visibility of traffic layer.
		/// </summary>
		public Visibility IsTrafficLayerVisible
		{
			get { return DataContext.MapOverlays.Contains(GoogleMapsLayer.TrafficOverlay) ? Visibility.Visible : Visibility.Collapsed; }
		}

		private const string IsTrafficLayerVisibleProperty = "IsTrafficLayerVisible";

		/// <summary>
		/// Gets the visibility of transit layer.
		/// </summary>
		public Visibility IsTransitLayerVisible
		{
			get { return DataContext.MapOverlays.Contains(GoogleMapsLayer.TransitOverlay) ? Visibility.Visible : Visibility.Collapsed; }
		}

		private const string IsTransitLayerVisibleProperty = "IsTransitLayerVisible";

		/// <summary>
		/// Gets the state of the toolbar.
		/// </summary>
		public ObservableValue<ExpansionStates> ToolbarState { get; private set; }

		#endregion


		#region Commands

		/// <summary>
		/// Gets the command that adds a user pin.
		/// </summary>
		public DelegateCommand<Location> CommandAddPlace { get; private set; }

		/// <summary>
		/// Gets the command that toggles the pushpin content state.
		/// </summary>
		public DelegateCommand<DataViewModel<Place>> CommandSelectPushpin { get; private set; }

		/// <summary>
		/// Gets the command that collapses the user pin.
		/// </summary>
		public DelegateCommand<DataViewModel<Place>> CommandDeselectPushpin { get; private set; }

		/// <summary>
		/// Gets the command deletes a user pin.
		/// </summary>
		public DelegateCommand<DataViewModel<Place>> CommandDeletePlace { get; private set; }

		/// <summary>
		/// Gets the command that pins a search result.
		/// </summary>
		public DelegateCommand<DataViewModel<Place>> CommandPinSearchResult { get; private set; }

		/// <summary>
		/// Gets the command that gets suggestions that based on the <see cref="SearchInput"/>.
		/// </summary>
		public DelegateCommand CommandGetSuggestions { get; private set; }

		/// <summary>
		/// Gets the command that performs the search based on the <see cref="SearchInput"/>.
		/// </summary>
		public DelegateCommand CommandSearch { get; private set; }

		/// <summary>
		/// Gets the command that starts tracking current location.
		/// </summary>
		public DelegateCommand CommandStartTrackingCurrentLocation { get; private set; }

		/// <summary>
		/// Gets the command that stops tracking current location.
		/// </summary>
		public DelegateCommand CommandStopTrackingCurrentLocation { get; private set; }

		/// <summary>
		/// Gets the command that sets the <see cref="VisualState"/> to <see cref="VisualStates.Search"/>.
		/// </summary>
		public DelegateCommand CommandGoToSearchState { get; private set; }

		/// <summary>
		/// Gets the command that sets the <see cref="VisualState"/> to <see cref="VisualStates.Default"/>.
		/// </summary>
		public DelegateCommand CommandGoToDefaultState { get; private set; }

		/// <summary>
		/// Gets the command that clears search results.
		/// </summary>
		public DelegateCommand CommandClearSearchResults { get; private set; }

		/// <summary>
		/// Gets the command that updates the information of a given place.
		/// </summary>
		public DelegateCommand<DataViewModel<Place>> CommandUpdatePlace { get; private set; }

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
		public DelegateCommand<GoogleMapsLayer> CommandToggleMapOverlay { get; private set; }

		/// <summary>
		/// Gets the command that toggles the <see cref="ToolbarState"/>.
		/// </summary>
		public DelegateCommand CommandToggleToolbar { get; private set; }

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when <see cref="CommandStopGeoWatcher"/> is executed.
		/// </summary>
		private void OnStopGeoWatcher()
		{
			_geoWatcher.Stop();
		}

		/// <summary>
		/// Called when <see cref="CommandStartGeoWatcher"/> is executed.
		/// </summary>
		private void OnStartGeoWatcher()
		{
			_geoWatcher.Start();
		}

		/// <summary>
		/// Called when <see cref="CommandGoToDefaultState"/>.
		/// </summary>
		private void OnGoToDefaultState()
		{
			DataContext.CancelGetSuggestions();
			VisualState.Value = VisualStates.Default;
		}

		/// <summary>
		/// Called when <see cref="CommandSelectPushpin"/> is executed.
		/// </summary>
		private void OnSelectPushpin(DataViewModel<Place> pushpin)
		{
			SelectedPushpin = pushpin;
		}

		/// <summary>
		/// Called when <see cref="CommandAddPlace"/> is executed.
		/// </summary>
		private void OnAddPlace(Location location)
		{
			DataContext.AddNewPlace(location);
		}

		/// <summary>
		/// Called when <see cref="CommandDeselectPushpin"/> is executed.
		/// </summary>
		private void OnDeselectPushpin(DataViewModel<Place> vm)
		{
			if (SelectedPushpin != vm)
			{
				return;
			}
			SelectedPushpin = null;
		}

		/// <summary>
		/// Called when <see cref="CommandDeletePlace"/> is executed.
		/// </summary>
		private void OnDeletePlace(DataViewModel<Place> vm)
		{
			DataContext.RemovePlace(vm.Data);
		}

		/// <summary>
		/// Called when <see cref="CommandPinSearchResult"/> is executed.
		/// </summary>
		private void OnPinSearchResult(DataViewModel<Place> vm)
		{
			vm.Data.IsSearchResult = false;
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
			ApplicationContext.IsBusy.Value = true;
			VisualState.Value = VisualStates.Default;
			DataContext.CancelGetSuggestions();
			DataContext.Search(Center, SearchInput, callback =>
			{
				ApplicationContext.IsBusy.Value = false;
				if (callback.Status != CallbackStatus.Successful)
				{
					const string messageBoxText = "Nothing was found in the search.";
					MessageBox.Show(messageBoxText);
					return;
				}
				IsTrackingCurrentLocation = false;
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

			ApplicationContext.IsBusy.Value = true;
			DataContext.GetSuggestions(Center, SearchInput, args =>
			{
				ApplicationContext.IsBusy.Value = false;
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

			ApplicationContext.IsBusy.Value = true;
			DataContext.GetPlaceDetails(SelectedSuggestion.Reference, args =>
			{
				ApplicationContext.IsBusy.Value = false;
				if (args.Status != CallbackStatus.Successful)
				{
					const string
						messageBoxText = "There was a problem getting information for your selected place, please try again later.",
						caption = "Unable to find place";
					MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK);
					return;
				}
				IsTrackingCurrentLocation = false;
				SearchSucceeded.ExecuteIfNotNull(new List<Place> { args.Result });
			});

			ResetSearchSuggestions();
		}

		/// <summary>
		/// Called when <see cref="CommandStartTrackingCurrentLocation"/> is executed.
		/// </summary>
		private void OnStopTrackingCurrentLocation()
		{
			IsTrackingCurrentLocation = false;
		}

		/// <summary>
		/// Called when <see cref="CommandStopTrackingCurrentLocation"/> is executed.
		/// </summary>
		private void OnStartTrackingCurrentLocation()
		{
			IsTrackingCurrentLocation = true;
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
			if (!IsTrackingCurrentLocation)
			{
				return;
			}
			Center = location;

			// This is to determine whether this is the first current location update.
			// The first update should set the zoom level to 15.
			if (oldCurrentLocation == null)
			{
				ZoomLevel = 15;
			}
		}

		/// <summary>
		/// Called when <see cref="CommandGoToSearchState"/> is executed.
		/// </summary>
		private void OnGoToSearchState()
		{
			VisualState.Value = VisualStates.Search;
		}

		/// <summary>
		/// Called when <see cref="CommandClearSearchResults"/> is executed.
		/// </summary>
		private void OnClearSearchResults()
		{
			DataContext.ClearSearchResults();
		}

		/// <summary>
		/// Called when <see cref="CommandUpdatePlace"/> is executed.
		/// </summary>
		/// <param name="pushpin">The pushpin view model.</param>
		private void OnUpdatePlace(DataViewModel<Place> pushpin)
		{
			DataContext.GetPlaceDetails(pushpin.Data);
		}

		/// <summary>
		/// Called when the value of <see cref="VisualState"/> is chagned.
		/// </summary>
		private void OnVisualStateChanged(VisualStates old, VisualStates @new)
		{
			if (old == VisualStates.Drag)
			{
				DragPushpin = null;
			}
		}

		#endregion


		#region Private Methods

		private void ResetSearchSuggestions()
		{
			SelectedSuggestion = null;
			_suggestions.Clear();
		}

		#endregion
	}
}
