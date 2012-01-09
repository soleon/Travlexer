using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls.Maps;
using Travlexer.WindowsPhone.Core.Collections;
using Travlexer.WindowsPhone.Core.Commands;
using Travlexer.WindowsPhone.Core.Extensions;
using Travlexer.WindowsPhone.Core.Services;
using Travlexer.WindowsPhone.Core.ViewModels;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services.GoogleMaps;
using Travlexer.WindowsPhone.Views;
using Place = Travlexer.WindowsPhone.Models.Place;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the view model for <see cref="MapView"/>
	/// </summary>
	public class MapViewModel : ViewModelBase
	{
		#region Public Enums

		public enum VisualStates : byte
		{
			Default = 0,
			Search,
			PushpinSelected
		}

		#endregion


		#region Private Fields

		private static readonly IDataContext _data = Globals.DataContext;

		#endregion


		#region Public Events

		/// <summary>
		/// Occurs when value of <see cref="SelectedPushpin"/> is changed.
		/// </summary>
		public event Action<PushpinViewModel> SelectedPushpinChanged;

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
			Pushpins = new AdaptedObservableCollection<Place, PushpinViewModel>(p => new PushpinViewModel(p, parent: this), _data.Places);
			Pushpins.CollectionChanged += OnPushpinsCollectionChanged;
			Suggestions = new ReadOnlyObservableCollection<SearchSuggestion>(_suggestions);

			CommandGetSuggestions = new DelegateCommand(OnGetSuggestions);
			CommandSearch = new DelegateCommand(OnSearch);
			CommandAddPlace = new DelegateCommand<Location>(OnAddPlace);
			CommandSelectPushpin = new DelegateCommand<PushpinViewModel>(OnSelectPushpin);
			CommandDeselectPushpin = new DelegateCommand<PushpinViewModel>(OnDeselectPushpin);
			CommandDeletePlace = new DelegateCommand<PushpinViewModel>(OnDeletePlace);
			CommandPinSearchResult = new DelegateCommand<PushpinViewModel>(OnPinSearchResult);
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
				if (Pushpins == null)
				{
					return;
				}
				foreach (var pushpin in Pushpins)
				{
					pushpin.VisualState = value == pushpin ? PushpinOverlayVisualStates.Expanded : PushpinOverlayVisualStates.Collapsed;
				}
				if (value != null)
				{
					MapCenter = value.Data.Location;
				}
				SelectedPushpinChanged.ExecuteIfNotNull(value);
			}
		}

		private PushpinViewModel _selectedPushpin;
		private const string SelectedPushpinProperty = "SelectedPushpin";

		/// <summary>
		/// Gets or sets the map center geo-coordination.
		/// </summary>
		public Location MapCenter
		{
			get { return _mapCenter; }
			set { SetProperty(ref _mapCenter, value, MapCenterProperty); }
		}

		private Location _mapCenter;
		private const string MapCenterProperty = "MapCenter";

		/// <summary>
		/// Sets the function that returns the current view port of the map.
		/// </summary>
		public Func<LocationRect> MapBoundCapturer { private get; set; }

		/// <summary>
		/// Gets the suggestions based on the <see cref="Input"/>.
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
				if (!SetProperty(ref _selectedSuggestion, value, SelectedSuggestionProperty) || value == null)
				{
					return;
				}
				OnSuggestionSelected(value);
			}
		}
		private SearchSuggestion _selectedSuggestion;
		private const string SelectedSuggestionProperty = "SelectedSuggestion";

		/// <summary>
		/// Gets or sets the search input.
		/// </summary>
		public string Input
		{
			get { return _input; }
			set { SetProperty(ref _input, value, InputProperty); }
		}

		private string _input;
		private const string InputProperty = "Input";

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
				_visualState = value;
				VisualStateChanged.ExecuteIfNotNull(value);
			}
		}

		private VisualStates _visualState;

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
		/// Gets the command that gets suggestions that based on the <see cref="Input"/>.
		/// </summary>
		public DelegateCommand CommandGetSuggestions { get; private set; }

		/// <summary>
		/// Gets the command that performs the search based on the <see cref="Input"/>.
		/// </summary>
		public DelegateCommand CommandSearch { get; private set; }

		#endregion


		#region Event Handling

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
			var place = _data.AddNewPlace(location);
			var vm = Pushpins.LastOrDefault();
			if (vm == null)
			{
				return;
			}
			vm.WorkingState = PushpinOverlayWorkingStates.Working;
			_data.GetPlaceInformation(
				place,
				args => vm.WorkingState = args.Status == CallbackStatus.Successful ? PushpinOverlayWorkingStates.Idle : PushpinOverlayWorkingStates.Error);
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
			_data.RemovePlace(vm.Data);
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
			Globals.DataContext.Search(MapCenter, Input, args =>
			{
				if (args.Status != CallbackStatus.Successful)
				{
					const string
						messageBoxText = "There was no result coming back form the search, please try again later.",
						caption = "Nothing Was Found";
					MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK);
					return;
				}
				var places = args.Result;
				SearchSucceeded.ExecuteIfNotNull(places);
			});
		}

		/// <summary>
		/// Called when <see cref="CommandGetSuggestions"/> is executed.
		/// </summary>
		private void OnGetSuggestions()
		{
			_data.GetSuggestions(MapCenter, Input, args =>
			{
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
		private void OnSuggestionSelected(SearchSuggestion suggestion)
		{

		}

		protected override void OnDispose()
		{
			Pushpins.CollectionChanged -= OnPushpinsCollectionChanged;
			Pushpins = null;
			SelectedPushpin = null;
			MapBoundCapturer = null;

			base.OnDispose();
		}

		#endregion
	}
}
