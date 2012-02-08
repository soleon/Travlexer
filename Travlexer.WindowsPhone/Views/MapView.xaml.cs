using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Codify;
using Codify.Controls.Maps;
using Codify.Extensions;
using Codify.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Core;
using Microsoft.Phone.Controls.Maps.Overlays;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.ViewModels;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Travlexer.WindowsPhone.Views
{
	public partial class MapView
	{
		#region Constants

		/// <summary>
		/// A fixed path to save the screen shot when the capture screen button is pressed.
		/// </summary>
		private const string PathScreenCapture = "Travlexer.jpg";

		private const int TileDimension = 256;

		#endregion


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
		private readonly ShadowText _debugText;
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
				_context.SuggestionsRetrieved += OnSuggestionsRetrieved;
				_context.VisualState.ValueChanged += (old, @new) => GoToVisualState(@new);
				_context.ToolbarState.ValueChanged += (old, @new) => GoToToolbarState(@new);

				GoToVisualState(_context.VisualState.Value, false);
				GoToToolbarState(_context.ToolbarState.Value, false);
			}

#if DEBUG
			if (DesignerProperties.IsInDesignTool)
			{
				return;
			}
			((Grid)Content).Children.Add(_debugText = new ShadowText { IsHitTestVisible = false, RenderTransform = new TranslateTransform { Y = 30 } });

			Map.MouseLeftButtonDown += (s, e) => { _debugText.Text = Map.ViewportPointToLocation(e.GetPosition(Map)).ToString(); };

			_context.PropertyChanged += (s, e) =>
			{
				switch (e.PropertyName)
				{
					case "ZoomLevel":
						_keys.Clear();
						OfflineLayer.Children.Clear();
						ShowOfflineArea();
						break;
					case "Center":
						ShowOfflineArea();
						break;
				}
			};

			Map.Loaded += (s, e) => ShowOfflineArea();
#endif
		}

		#endregion

#if DEBUG
		private readonly List<QuadKey> _keys = new List<QuadKey>();
		private QuadKey ToQuadKey(Location location, int zoomLevel)
		{
			var tileNumber1D = Math.Floor(Math.Pow(2D, zoomLevel));
			var quadMapDimension = TileDimension * tileNumber1D / 2;
			var geoCenterPoint = Map.LocationToViewportPoint(new GeoCoordinate(0, 0));
			var geoCenterPointX = geoCenterPoint.X;
			var geoCenterPointY = geoCenterPoint.Y;
			var currentPoint = Map.LocationToViewportPoint(location);
			var currentPointX = currentPoint.X;
			var currentPointY = currentPoint.Y;
			var mapEdgeToScreenEdgeX = quadMapDimension - geoCenterPointX;
			var mapEdgeToScreenEdgeY = quadMapDimension - geoCenterPointY;
			var mapEdgeToCurrentPointX = mapEdgeToScreenEdgeX + currentPointX;
			var mapEdgeToCurrentPointY = mapEdgeToScreenEdgeY + currentPointY;

			var x = mapEdgeToCurrentPointX / TileDimension;
			var y = mapEdgeToCurrentPointY / TileDimension;
			return new QuadKey((int)x, (int)y, zoomLevel);
		}

		private void ShowOfflineArea()
		{
			var center = Map.Center;
			var z = (int)Math.Floor(Map.ZoomLevel);
			var key = ToQuadKey(center, z);
			var x = key.X;
			var y = key.Y;
			AddOfflineTile(x, y, z, GoogleMapsLayer.Street);
			AddOfflineTile(x, GetRealTileNumber(y + 1, z), z, GoogleMapsLayer.Street);
			AddOfflineTile(GetRealTileNumber(x + 1, z), y, z, GoogleMapsLayer.Street);
			AddOfflineTile(GetRealTileNumber(x + 1, z), GetRealTileNumber(y + 1, z), z, GoogleMapsLayer.Street);
			AddOfflineTile(GetRealTileNumber(x - 1, z), y, z, GoogleMapsLayer.Street);
			AddOfflineTile(x, GetRealTileNumber(y - 1, z), z, GoogleMapsLayer.Street);
			AddOfflineTile(GetRealTileNumber(x - 1, z), GetRealTileNumber(y - 1, z), z, GoogleMapsLayer.Street);
			AddOfflineTile(GetRealTileNumber(x + 1, z), GetRealTileNumber(y - 1, z), z, GoogleMapsLayer.Street);
			AddOfflineTile(GetRealTileNumber(x - 1, z), GetRealTileNumber(y + 1, z), z, GoogleMapsLayer.Street);
		}

		private void AddOfflineTile(int x, int y, int z, GoogleMapsLayer layer)
		{
			var key = new QuadKey(x, y, z);
			if (_keys.Contains(key))
			{
				return;
			}
			var tailNumber1D = (int)Math.Floor(Math.Pow(2D, z));
			var uri = GoogleMapsTileSource.GetUri(x, y, z, layer);
			var tailNumberToGeoCenter = tailNumber1D / 2;
			var modifierX = x >= tailNumberToGeoCenter ? 1 : -1;
			var modifierY = y >= tailNumberToGeoCenter ? 1 : -1;
			var xNumberToGeoCenter = Math.Abs(x - tailNumberToGeoCenter);
			var yNumberToGeoCenter = Math.Abs(y - tailNumberToGeoCenter);
			var geoCenterPoint = Map.LocationToViewportPoint(new GeoCoordinate(0, 0));
			var geoCenterPointX = geoCenterPoint.X;
			var geoCenterPointY = geoCenterPoint.Y;

			var tailPointX = geoCenterPointX + (TileDimension * xNumberToGeoCenter * modifierX);
			var tailPointY = geoCenterPointY + (TileDimension * yNumberToGeoCenter * modifierY);
			var tailPoint = new Point(tailPointX, tailPointY);
			var tailLocation = Map.ViewportPointToLocation(tailPoint);

			var tilePushpin = new Pushpin
			{
				PositionOrigin = PositionOrigin.TopLeft,
				Template = null,
				Content = new Rectangle
				{
					Width = TileDimension,
					Height = TileDimension,
					Fill = new ImageBrush { ImageSource = new BitmapImage(uri) }
				},
				Location = tailLocation
			};
			OfflineLayer.Children.Add(tilePushpin);
			_keys.Add(key);
		}

		private int GetRealTileNumber(int logicalTileNumber, int zoomLevel)
		{
			var tileNumber1D = (int)Math.Floor(Math.Pow(2D, zoomLevel));
			if (logicalTileNumber < 0)
			{
				return tileNumber1D + logicalTileNumber;
			}
			if (logicalTileNumber >= tileNumber1D)
			{
				return logicalTileNumber - tileNumber1D;
			}
			return logicalTileNumber;
		}
#endif


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
				var coordinates = places.Select(p => (GeoCoordinate)p.Location).ToArray();
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
			if (_context.VisualState.Value != MapViewModel.VisualStates.Default)
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

		/// <summary>
		/// Tries to capture and save the current screen to the media library.
		/// </summary>
		private void OnCaptureScreen(object sender, RoutedEventArgs e)
		{
			try
			{
				var bmp = new WriteableBitmap(Map, null);
				using (var ms = new MemoryStream())
				{
					bmp.SaveJpeg(ms, (int)ActualWidth, (int)ActualHeight, 0, 100);
					ms.Seek(0, SeekOrigin.Begin);

					var lib = new MediaLibrary();
					lib.SavePicture(PathScreenCapture, ms);
				}

				const string messageBoxText = "A screenshot is saved in your media library.";
				const string caption = "Screenshot Saved";
				MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK);
			}
			catch
			{
				const string messageBoxText = "There was an error saving the screenshot. Please disconnect your phone from the computer or make sure you phone's storage is not full.";
				const string caption = "Error Saving Screenshot";
				MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK);
			}
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
			var transform = (CompositeTransform)DragPushpin.RenderTransform;
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
			var transform = (CompositeTransform)DragPushpin.RenderTransform;
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
			var pushpin = (Pushpin)sender;
			var data = (DataViewModel<Place>)pushpin.DataContext;
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
		private void GoToVisualState(MapViewModel.VisualStates state, bool useTransition = true)
		{
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
				case MapViewModel.VisualStates.Drag:
					_appBar.IsVisible = false;
					break;
			}
			VisualStateManager.GoToState(this, state.ToString(), useTransition);
		}

		/// <summary>
		/// Sets the state of to toolbar.
		/// </summary>
		private void GoToToolbarState(ExpansionStates state, bool useTransition = true)
		{
			VisualStateManager.GoToState(this, state.ToString(), useTransition);
		}

		#endregion
	}
}
