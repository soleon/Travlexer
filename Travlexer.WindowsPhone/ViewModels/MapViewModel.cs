using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Device.Location;
using System.Linq;
using System.Windows;
using Codify.WindowsPhone;
using Codify.WindowsPhone.Collections;
using Codify.WindowsPhone.Commands;
using Codify.WindowsPhone.Extensions;
using Codify.WindowsPhone.Services;
using Codify.WindowsPhone.Threading;
using Codify.WindowsPhone.ViewModels;
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
		/// Occurs when <see cref="VisualState"/> is changed.
		/// </summary>
		public event Action<VisualStates> VisualStateChanged;

		/// <summary>
		/// Occurs when <see cref="Suggestions"/> are retrieved from service client.
		/// </summary>
		public event Action SuggestionsRetrieved;

		#endregion


		#region Constructors

		public MapViewModel()
		{
			Center = DataContext.MapCenter;
			ZoomLevel = DataContext.MapZoomLevel;
			SearchInput = DataContext.SearchInput;
			IsTrackingCurrentLocation = DataContext.IsTrackingCurrentLocation;

			Suggestions = new ReadOnlyObservableCollection<SearchSuggestion>(_suggestions);
			Pushpins = new AdaptedObservableCollection<Place, PushpinViewModel>(p => new PushpinViewModel(p, this), DataContext.Places);
			Pushpins.CollectionChanged += OnPushpinsCollectionChanged;

			CommandGetSuggestions = new DelegateCommand(OnGetSuggestions);
			CommandSearch = new DelegateCommand(OnSearch);
			CommandAddPlace = new DelegateCommand<Location>(OnAddPlace);
			CommandSelectPushpin = new DelegateCommand<PushpinViewModel>(OnSelectPushpin);
			CommandDeselectPushpin = new DelegateCommand<PushpinViewModel>(OnDeselectPushpin);
			CommandDeletePlace = new DelegateCommand<PushpinViewModel>(OnDeletePlace);
			CommandPinSearchResult = new DelegateCommand<PushpinViewModel>(OnPinSearchResult);
			CommandUpdatePlace = new DelegateCommand<PushpinViewModel>(OnUpdatePlace);
			CommandStartTrackingCurrentLocation = new DelegateCommand(OnStartTrackingCurrentLocation);
			CommandStopTrackingCurrentLocation = new DelegateCommand(OnStopTrackingCurrentLocation);
			CommandGoToSearchState = new DelegateCommand(OnGoToSearchState);
			CommandGoToDefaultState = new DelegateCommand(OnGoToDefaultState);
			CommandClearSearchResults = new DelegateCommand(OnClearSearchResults);
			CommandStartGeoWatcher = new DelegateCommand(OnStartGeoWatcher);
			CommandStopGeoWatcher = new DelegateCommand(OnStopGeoWatcher);
			CommandAddCurrentPlace = new DelegateCommand(() => OnAddPlace(CurrentLocation), () => CurrentLocation != null && !CurrentLocation.IsUnknown);

			_geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 10D };
			_geoWatcher.PositionChanged += OnGeoWatcherPositionChanged;
			if (DataContext.IsTrackingCurrentLocation)
			{
				if (!_geoWatcher.Position.Location.IsUnknown)
				{
					Center = _geoWatcher.Position.Location;
				}
				_geoWatcher.Start();
			}

			ApplicationContext.IsBusy.ValueChanged += (oldValue, newValue) => RaisePropertyChange(IsBusyProperty);
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection of all user pins.
		/// </summary>
		public AdaptedObservableCollection<Place, PushpinViewModel> Pushpins { get; private set; }

		/// <summary>
		/// Gets or sets the selected place.
		/// </summary>
		public PushpinViewModel SelectedPushpin
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
					VisualState = VisualStates.Default;
				}
				else
				{
					DragPushpin = null;
					var place = value.Data;
					Center = place.Location;
					if (place.DataState != DataStates.Loaded)
					{
						OnUpdatePlace(value);
					}
					VisualState = VisualStates.PushpinSelected;

				}
			}
		}

		private PushpinViewModel _selectedPushpin;
		private const string SelectedPushpinProperty = "SelectedPushpin";

		/// <summary>
		/// Gets or sets the pushpin that's been dragged.
		/// </summary>
		public PushpinViewModel DragPushpin
		{
			get { return _dragPushpin; }
			set
			{
				var old = _dragPushpin;
				if (SetProperty(ref _dragPushpin, value, DragPushpinProperty))
				{
					if (old != null)
					{
						old.IsDragging = false;
					}
					if (value == null)
					{
						VisualState = VisualStates.Default;
					}
					else
					{
						SelectedPushpin = null;
						value.IsDragging = true;
						VisualState = VisualStates.Drag;
						IsTrackingCurrentLocation = false;
					}
				}
			}
		}

		private PushpinViewModel _dragPushpin;
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
				if (DataContext.MapZoomLevel.Equals(value))
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
		/// Gets or sets the visual state for the view.
		/// </summary>
		public VisualStates VisualState
		{
			get { return _visualState; }
			set
			{
				if (_visualState == value)
				{
					return;
				}
				var old = _visualState;
				_visualState = value;
				if (old == VisualStates.Drag)
				{
					DragPushpin = null;
				}
				VisualStateChanged.ExecuteIfNotNull(value);
			}
		}

		private VisualStates _visualState;

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
					if (ZoomLevel < 15)
					{
						ZoomLevel = 15;
					}
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

		#endregion


		#region Commands

		/// <summary>
		/// Gets the command that adds a user pin.
		/// </summary>
		public DelegateCommand<Location> CommandAddPlace { get; private set; }

		/// <summary>
		/// Gets the command that toggles the pushpin content state.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandSelectPushpin { get; private set; }

		/// <summary>
		/// Gets the command that collapses the user pin.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandDeselectPushpin { get; private set; }

		/// <summary>
		/// Gets the command deletes a user pin.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandDeletePlace { get; private set; }

		/// <summary>
		/// Gets the command that pins a search result.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandPinSearchResult { get; private set; }

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
		public DelegateCommand<PushpinViewModel> CommandUpdatePlace { get; private set; }

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
			VisualState = VisualStates.Default;
		}

		/// <summary>
		/// Called when <see cref="CommandSelectPushpin"/> is executed.
		/// </summary>
		private void OnSelectPushpin(PushpinViewModel pushpin)
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
		private void OnDeselectPushpin(PushpinViewModel vm)
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
		private void OnDeletePlace(PushpinViewModel vm)
		{
			DataContext.RemovePlace(vm.Data);
		}

		/// <summary>
		/// Called when <see cref="CommandPinSearchResult"/> is executed.
		/// </summary>
		private void OnPinSearchResult(PushpinViewModel vm)
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
					foreach (var pushpin in e.OldItems.Cast<PushpinViewModel>())
					{
						if (SelectedPushpin == pushpin)
						{
							SelectedPushpin = null;
						}
						pushpin.Dispose();
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
			VisualState = VisualStates.Default;
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
			if (VisualState != VisualStates.Search)
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
			UIThread.InvokeBack(() => VisualState = VisualStates.Default);

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
			CurrentLocation = location;
			if (!IsTrackingCurrentLocation)
			{
				return;
			}
			Center = location;
		}

		/// <summary>
		/// Called when <see cref="CommandGoToSearchState"/> is executed.
		/// </summary>
		private void OnGoToSearchState()
		{
			VisualState = VisualStates.Search;
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
		private void OnUpdatePlace(PushpinViewModel pushpin)
		{
			DataContext.GetPlaceDetails(pushpin.Data);
		}

		protected override void OnDispose()
		{
			Pushpins.CollectionChanged -= OnPushpinsCollectionChanged;
			Pushpins = null;
			SelectedPushpin = null;

			base.OnDispose();
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
