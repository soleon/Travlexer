using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codify.WindowsPhone.Extensions;
using Codify.WindowsPhone.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using Travlexer.WindowsPhone.Infrastructure.Models;
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

#if	DEBUG
		private readonly TextBlock _debugText;
#endif

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

#if DEBUG
			if (!DesignerProperties.IsInDesignTool)
			{
				Map.Children.Add(_debugText = new TextBlock { Foreground = new SolidColorBrush(Colors.Red), IsHitTestVisible = false, RenderTransform = new TranslateTransform { Y = 30 } });
				_debugText.Text = "debug";
			}
#endif
		}

		#endregion


		#region Event Handling

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.Hold"/> event occurs.
		/// </summary>
		protected override void OnHold(GestureEventArgs e)
		{
			var coordinate = Map.ViewportPointToLocation(e.GetPosition(Map));
			_context.CommandAddPlace.Execute(coordinate);
			base.OnHold(e);
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
					catch (ArgumentOutOfRangeException)
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

		private void OnCancelMapPan(object sender, MapDragEventArgs e)
		{
			e.Handled = true;
		}

		private void OnPushpinDragStarted(object sender, DragStartedGestureEventArgs e)
		{
			e.Handled = true;
			if (_context.DragPushpin == null)
			{
				return;
			}
			Map.MouseLeftButtonUp -= OnPushpinRelease;
			Map.MapPan -= OnCancelMapPan;
			Map.MapPan += OnCancelMapPan;
		}

		private void OnPushpinDragDelta(object sender, DragDeltaGestureEventArgs e)
		{
			e.Handled = true;
			if (_context.DragPushpin == null)
			{
				return;
			}
			var transform = (CompositeTransform) DragPushpin.RenderTransform;
			transform.TranslateX += e.HorizontalChange;
			transform.TranslateY += e.VerticalChange;
		}

		private void OnPushpinDragCompleted(object sender, DragCompletedGestureEventArgs e)
		{
			e.Handled = true;

			// Only react if there's a pushpin being dragged.
			var dragPushpinVm = _context.DragPushpin;
			if (dragPushpinVm == null)
			{
				return;
			}

			// Calculate the drop point to geo-coordinates.
			var point = DragPushpin.TransformToVisual(Map).Transform(new Point());
			point.X += DragPushpin.ActualWidth / 2;
			point.Y += DragPushpin.ActualHeight;
			var location = Map.ViewportPointToLocation(point);
			_context.Center = dragPushpinVm.Data.Location = location;

			// Select the dragged pushpin.
			_context.SelectedPushpin = dragPushpinVm;

			// Reset transformation of the drag cue.
			var transform = (CompositeTransform) DragPushpin.RenderTransform;
			transform.TranslateX = 0;
			transform.TranslateY = 0;

			// Unhook events.
			Map.MouseLeftButtonUp -= OnPushpinRelease;
			Map.MapPan -= OnCancelMapPan;

			// Update information for new location.
			_context.CommandUpdatePlace.ExecuteIfNotNull(dragPushpinVm);
		}

		private void OnPushpinRelease(object sender, MouseButtonEventArgs e)
		{
			Map.MouseLeftButtonUp -= OnPushpinRelease;
			_context.DragPushpin = null;
		}

		private void OnPushpinHold(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
		{
			var pushpin = (Pushpin) sender;
			var data = (DataViewModel<Place>) pushpin.DataContext;
			if (data.Data.IsSearchResult)
			{
				return;
			}
			_context.DragPushpin = data;
			Map.MouseLeftButtonUp -= OnPushpinRelease;
			Map.MouseLeftButtonUp += OnPushpinRelease;
		}

		private void OnMapPinchStarted(object sender, PinchStartedGestureEventArgs e)
		{
			_context.CommandStopTrackingCurrentLocation.ExecuteIfNotNull();
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
