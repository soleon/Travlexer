using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.ViewModels;

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
				_context.SearchSucceeded += OnSearchSucceeded;
				_context.VisualStateChanged += GoToState;
				_context.SuggestionsRetrieved += OnSuggestionsRetrieved;
			}

			// The "Hold" event in XAML is not recognised by Blend.
			// Doing event handling here instead of in XAML is a hack to make the view still "blendable".
			Map.Hold += OnMapHold;
		}

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when <see cref="UIElement.Hold"/> event is raise on the map.
		/// </summary>
		private void OnMapHold(object sender, GestureEventArgs e)
		{
			var coordinate = Map.ViewportPointToLocation(e.GetPosition(Map));
			_context.CommandAddPlace.Execute(coordinate);
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
					try
					{
						Map.SetView(place.ViewPort);
					}
					catch (System.ArgumentOutOfRangeException)
					{
						Map.SetView(place.Location, 15D);
					}
				}
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

		/// <summary>
		/// This method is called when the hardware Back button is pressed.
		/// Cancel the back key press if the view is not in default visual state.
		/// </summary>
		protected override void OnBackKeyPress(CancelEventArgs e)
		{
			if (_context.VisualState != MapViewModel.VisualStates.Default)
			{
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Called when <see cref="MapViewModel.SuggestionsRetrieved"/> event is raised from the data context of this view.
		/// </summary>
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
			switch (state)
			{
				case MapViewModel.VisualStates.Default:
					Map.Focus();
					_appBar.IsVisible = true;
					break;
				case MapViewModel.VisualStates.Search:
					SearchBox.Focus();
					_appBar.IsVisible = false;
					break;
				case MapViewModel.VisualStates.PushpinSelected:
					_appBar.IsVisible = false;
					break;
			}
			VisualStateManager.GoToState(this, state.ToString(), true);
		}

		#endregion
	}
}
