using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using Travlexer.WindowsPhone.Core.Extensions;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.ViewModels;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Travlexer.WindowsPhone.Views
{
	public partial class MapView
	{
		#region Private Members

		/// <summary>
		/// A private shortcut to the current data context.
		/// </summary>
		private readonly MapViewModel _context;

		/// <summary>
		/// A reference to the current <see cref="ApplicationBar"/>.
		/// </summary>
		private readonly IApplicationBar _appBar;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MapView"/> class.
		/// </summary>
		public MapView()
		{
			InitializeComponent();
			_context = DataContext as MapViewModel;
			_appBar = ApplicationBar;

			if (_context != null)
			{
				_context.MapBoundCapturer = OnCaptureMapBound;
				_context.SelectedPushpinChanged += OnSelectedPushpinChanged;
				_context.SearchSucceeded += OnSearchSucceeded;
				_context.VisualStateChanged += OnVisualStateChanged;
				_context.SuggestionsRetrieved += OnSuggestionsRetrieved;
			}

			// The "Hold" event in XAML is not recognised by Blend.
			// Doing event handling here instead of in XAML is a hack to make the view still "blendable".
			Map.Hold += OnMapHold;
		}


		#endregion


		#region Event Handling

		/// <summary>
		/// Called when the selected pushpin of the data context is changed.
		/// </summary>
		/// <param name="vm">The vm.</param>
		private void OnSelectedPushpinChanged(PushpinViewModel vm)
		{
			GoToState(vm == null ? MapViewModel.VisualStates.Default : MapViewModel.VisualStates.PushpinSelected);
		}

		/// <summary>
		/// Called when <see cref="UIElement.Hold"/> event is raise on the map.
		/// </summary>
		private void OnMapHold(object sender, GestureEventArgs e)
		{
			var coordinate = Map.ViewportPointToLocation(e.GetPosition(Map));
			_context.CommandAddPlace.ExecuteIfNotNull(coordinate);
		}

		/// <summary>
		/// Called when the current view port of the map is required.
		/// </summary>
		private LocationRect OnCaptureMapBound()
		{
			return Map.BoundingRectangle;
		}


		/// <summary>
		/// Called when <see cref="AppButtonSearch"/> is clicked.
		/// </summary>
		private void OnAppButtonSearchClick(object sender, EventArgs e)
		{
			GoToState(MapViewModel.VisualStates.Search);
			SearchBox.Focus();
		}

		/// <summary>
		/// Called when <see cref="MapViewModel.SearchSucceeded"/> event is raised.
		/// Sets the map's view port to display the search results.
		/// </summary>
		/// <param name="places">The list of places retrived by the search.</param>
		private void OnSearchSucceeded(IList<Place> places)
		{
			if (places.Count == 1)
			{
				var place = places[0];
				var viewPort = place.ViewPort;
				if (viewPort == null)
				{
					Map.SetView(place.Location, 15D);
				}
				else
				{
					Map.SetView(places[0].ViewPort);
				}
			}
			else
			{
				var coordinates = places.Select(p => (GeoCoordinate)p.Location).ToArray();
				if (coordinates.Length == 0)
				{
					return;
				}
				Map.SetView(LocationRect.CreateLocationRect(coordinates));
			}
		}


		/// <summary>
		/// Called when the visual state of the view model has changed.
		/// </summary>
		private void OnVisualStateChanged(MapViewModel.VisualStates state)
		{
			if (state == MapViewModel.VisualStates.Default || state == MapViewModel.VisualStates.PushpinSelected)
			{
				_appBar.IsVisible = true;
				Map.Focus();
			}
			else
			{
				_appBar.IsVisible = false;
			}
			VisualStateManager.GoToState(this, state.ToString(), true);
		}

		/// <summary>
		/// This method is called when the hardware Back button is pressed.
		/// Returns to the Default visual state if the view is in another state, otherwise, exits the application.
		/// </summary>
		protected override void OnBackKeyPress(CancelEventArgs e)
		{
			if (_context.VisualState == MapViewModel.VisualStates.Default || _context.VisualState == MapViewModel.VisualStates.PushpinSelected)
			{
				base.OnBackKeyPress(e);
			}
			else
			{
				e.Cancel = true;
				GoToState(MapViewModel.VisualStates.Default);
			}
		}

		private void OnSuggestionsRetrieved()
		{
			SearchBox.PopulateComplete();
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Sets the <see cref="MapViewModel.VisualState"/> and focuses on the map control if the state is Default.
		/// </summary>
		private void GoToState(MapViewModel.VisualStates state)
		{
			_context.VisualState = state;
			if (state == MapViewModel.VisualStates.Default)
			{
				Map.Focus();
			}
		}

		#endregion

	}
}
