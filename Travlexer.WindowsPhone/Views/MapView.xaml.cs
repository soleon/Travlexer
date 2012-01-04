using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using Travlexer.WindowsPhone.Core.Extensions;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
	public partial class MapView
	{
		#region Constants

		private const string SearchStateName = "Search";
		private const string DefaultStateName = "Default";
		private const string PushpinSelectedStateName = "PushpinSelected";

		#endregion


		#region Private Members

		/// <summary>
		/// A private shortcut to the current data context.
		/// </summary>
		private readonly MapViewModel _context;

		#endregion


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MapView"/> class.
		/// </summary>
		public MapView()
		{
			InitializeComponent();
			_context = DataContext as MapViewModel;
			if (_context != null)
			{
				_context.MapBoundCapturer = OnCaptureMapBound;
				_context.SelectedPushpinChanged += OnSelectedPushpinChanged;
				_context.SearchSucceeded += OnSearchSucceeded;
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
			VisualStateManager.GoToState(this, vm == null ? DefaultStateName : PushpinSelectedStateName, true);
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
			VisualStateManager.GoToState(this, SearchStateName, true);
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
				var viewPort = places[0].ViewPort;
				if (viewPort == null)
				{
					return;
				}
				Map.SetView(places[0].ViewPort);
			}
			else
			{
				var coordinates = places.Select(p => (GeoCoordinate) p.Location).ToArray();
				if (coordinates.Length == 0)
				{
					return;
				}
				Map.SetView(LocationRect.CreateLocationRect(coordinates));
			}
		}

		#endregion
	}
}
