using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Codify;
using Codify.Controls.Maps;
using Codify.Extensions;
using Codify.Threading;
using Codify.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.ViewModels;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using QuadKey = Codify.Controls.Maps.QuadKey;

namespace Travlexer.WindowsPhone.Views
{
	public partial class MapView
	{
#if	DEBUG
		private readonly TextBlock _debugText;
#endif


		#region Constants

		private const string
			ScreenCapturePath = "Travlexer.jpg",
			OfflineTileImagePrefix = "t_",
			OfflineTilesSearchPattern = OfflineTileImagePrefix + "*",
			MapTilePushpinStyleName = "TilePushpin",
			MapTileScaleName = "TileScale",
			TileEmptyBackgroundPath = "/Assets/EmptyTile.png";

		private const int AbsoluteTileDimension = 256;

		#endregion


		#region Private Members

		private readonly MapViewModel _context;
		private readonly IApplicationBar _appBar;
		private readonly Dictionary<int, List<QuadKey>> _cachedTileImageKeys = new Dictionary<int, List<QuadKey>>();
		private readonly Style _tilePushpinStyle;		private readonly ScaleTransform _tileScaleTransform;
		private readonly BitmapImage _tileEmptyBackground;

		private readonly IsolatedStorageFile _fileStore = IsolatedStorageFile.GetUserStoreForApplication();
		private readonly GeoCoordinate _centerGeoCoordinate = new GeoCoordinate(0D, 0D);
		private readonly DispatcherTimer _zoomTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500D) };
		private readonly Queue<KeyValuePair<QuadKey, BitmapImage>> _tileImageCache = new Queue<KeyValuePair<QuadKey, BitmapImage>>(50);
		private bool _isZooming, _isOfflineModeInitialized;
		private double _tileScale;
		private int _zoomFloor, _tileMatrixLengthX, _tileMatrixLengthY;
		private Pushpin[,] _tileMatrix;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MapView"/> class.
		/// </summary>
		public MapView()
		{
			InitializeComponent();
#if DEBUG
			if (DesignerProperties.IsInDesignTool)
			{
				return;
			}
			((Grid )Content).Children.Add(_debugText = new TextBlock { IsHitTestVisible = false, RenderTransform = new TranslateTransform { Y = 30 }, Foreground = new SolidColorBrush(Colors.Red), Text = "debug" });
#endif

			_context = DataContext as MapViewModel;
			_appBar = ApplicationBar;

			if (_context == null)
			{
				return;
			}
			_context.SearchSucceeded += OnSearchSucceeded;
			_context.SuggestionsRetrieved += OnSuggestionsRetrieved;
			_context.VisualState.ValueChanged += (old, @new) => GoToVisualState(@new);
			_context.ToolbarState.ValueChanged += (old, @new) => GoToToolbarState(@new);
			_context.IsOffline.ValueChanged += (old, @new) =>
			{
				if (@new)
				{
					if (!_isOfflineModeInitialized)
					{
						InitializeOfflineMode();
					}
					EnterOfflineMode();
				}
				else
				{
					LeaveOfflineMode();
				}
			};

			GoToVisualState(_context.VisualState.Value, false);
			GoToToolbarState(_context.ToolbarState.Value, false);


			_tilePushpinStyle = (Style )Resources[MapTilePushpinStyleName];
			_tileScaleTransform = (ScaleTransform )Resources[MapTileScaleName];
			_tileEmptyBackground = new BitmapImage(new Uri(TileEmptyBackgroundPath, UriKind.Relative));
			_tileScale = 1D;

			if (_context.IsOffline.Value)
			{
				Map.Loaded += (s, e) =>
				{
					InitializeOfflineMode();
					EnterOfflineMode();
				};
			}
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
				var coordinates = places.Select(p => (GeoCoordinate )p.Location).ToArray();
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
					bmp.SaveJpeg(ms, (int )ActualWidth, (int )ActualHeight, 0, 100);
					ms.Seek(0, SeekOrigin.Begin);

					var lib = new MediaLibrary();
					lib.SavePicture(ScreenCapturePath, ms);
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
			var transform = (CompositeTransform )DragPushpin.RenderTransform;
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
			_context.Center.Value = dragPushpinVm.Data.Location = location;

			// Select the dragged pushpin.
			_context.SelectedPushpin = dragPushpinVm;

			// Reset transformation of the drag cue.
			var transform = (CompositeTransform )DragPushpin.RenderTransform;
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
			var pushpin = (Pushpin )sender;
			var data = (DataViewModel<Place> )pushpin.DataContext;
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

		/// <summary>
		/// Called when the zoom timer ticks. This re-arranges all offline tiles according to the current center point and zoom level.
		/// </summary>
		private void OnZoomTimerTick(object sender, EventArgs eventArgs)
		{
			_zoomTimer.Stop();

			var oldZoomFloor = _zoomFloor;
			_zoomFloor = (int )Map.ZoomLevel;
			_tileScaleTransform.ScaleX = _tileScaleTransform.ScaleY = _tileScale = Math.Pow(2D, (Map.ZoomLevel - _zoomFloor));
			if (oldZoomFloor == _zoomFloor)
			{
				return;
			}

			// Update all tiles if the zoom level has reached a new floor.
			var centerKey = GetQuadKey(Map.Center, _zoomFloor, default(GoogleMapsLayer));

			// Calculate X index and Y index in the matrix of the center tile.
			var centerX = _tileMatrixLengthX / 2;
			var centerY = _tileMatrixLengthY / 2;

			// Update location and image of tile pushpins.
			for (var x = 0; x < _tileMatrixLengthX; x++)
			{
				for (var y = 0; y < _tileMatrixLengthY; y++)
				{
					// Create the tile pushpin.
					var tile = _tileMatrix[x, y];

					// Calculate the differences between this tile's X/Y index and the center tile's X/Y index.
					var diffX = x - centerX;
					var diffY = y - centerY;

					// Calculate the quad key for this current tile.
					var key = new QuadKey(centerKey.X + diffX, centerKey.Y + diffY, _zoomFloor, centerKey.Layer);

					// Update this tile with the calculated quad key.
					UpdateTile(tile, key);
				}
			}

			_isZooming = false;
		}

		/// <summary>
		/// Updates the tile scaling ratio whenever the zoom level changes.
		/// </summary>
		private void OnZoomLevelChanged(double old, double @new)
		{
			_isZooming = true;
			UpdateTileScale();
			_zoomTimer.Start();
		}

		/// <summary>
		/// Performs necessary re-arangement to the offline tiles whenever the map moves.
		/// </summary>
		private void OnCenterChanged(GeoCoordinate geoCoordinate, GeoCoordinate coordinate)
		{
			if (_isZooming)
			{
				return;
			}

			var maxTileIndex = GetTileCount(_zoomFloor) - 1;
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;

			// Skip arrangement if the matrix is larger than the entire map.
			if (maxIndexX >= maxTileIndex && maxIndexY >= maxTileIndex)
			{
				return;
			}

			// Check representative edge tiles to see if they are fully moved into view.

			// Top left:
			var tile = _tileMatrix[0, 0];
			var key = (QuadKey )tile.Tag;
			var y = key.Y;
			if (IsInView(tile))
			{
				RearrangeMatrixRightToLeft(updateLastTileImage: y == 0);
				if (y > 0)
				{
					RearrangeMatrixBottomToTop();
				}
				return;
			}

			// Top right:
			tile = _tileMatrix[maxIndexX, 0];
			key = (QuadKey )tile.Tag;
			y = key.Y;
			if (IsInView(_tileMatrix[maxIndexX, 0]))
			{
				RearrangeMatrixLeftToRight(updateLastTileImage: y == 0);
				if (y > 0)
				{
					RearrangeMatrixBottomToTop();
				}
				return;
			}

			// Bottom left:
			tile = _tileMatrix[0, maxIndexY];
			key = (QuadKey )tile.Tag;
			y = key.Y;
			if (IsInView(tile))
			{
				if (y < maxTileIndex)
				{
					RearrangeMatrixTopToBottom(updateLastTileImage: false);
				}
				RearrangeMatrixRightToLeft();
				return;
			}

			// Bottom right:
			tile = _tileMatrix[maxIndexX, maxIndexY];
			key = (QuadKey )tile.Tag;
			y = key.Y;
			if (IsInView(tile))
			{
				if (y < maxTileIndex)
				{
					RearrangeMatrixTopToBottom(false);
				}
				RearrangeMatrixLeftToRight();
				return;
			}

			// Top:
			tile = _tileMatrix[maxIndexX / 2 + maxIndexX % 2, 0];
			key = (QuadKey )tile.Tag;
			y = key.Y;
			if (y > 0 && IsVerticalInView(tile))
			{
				RearrangeMatrixBottomToTop();
				return;
			}

			// Bottom:
			tile = _tileMatrix[maxIndexX / 2 + maxIndexX % 2, maxIndexY];
			key = (QuadKey )tile.Tag;
			y = key.Y;
			if (y < maxTileIndex && IsVerticalInView(tile))
			{
				RearrangeMatrixTopToBottom();
				return;
			}

			// Left:
			tile = _tileMatrix[0, maxIndexY / 2 + maxIndexY % 2];
			if (IsHorizontalInView(tile))
			{
				RearrangeMatrixRightToLeft();
				return;
			}

			// Right:
			tile = _tileMatrix[maxIndexX, maxIndexY / 2 + maxIndexY % 2];
			if (IsHorizontalInView(tile))
			{
				RearrangeMatrixLeftToRight();
			}
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

		/// <summary>
		/// Performs all necessary initialization for offline mapping. Happens only once.
		/// </summary>
		private void InitializeOfflineMode()
		{
			if (_isOfflineModeInitialized)
			{
				return;
			}

			// Get the zoom level floor from the current map zoom level.
			_zoomFloor = (int )Map.ZoomLevel;

			// Update the tile scaling ratio according to the current map zoom level.
			UpdateTileScale();

			// Construct the in-memory list of quad keys that represents the cached tile images.
			var keys = _fileStore.GetFileNames(OfflineTilesSearchPattern).Select(f => new QuadKey(f.Substring(OfflineTileImagePrefix.Length)));
			foreach (var key in keys)
			{
				GetCachedKeys(key.ZoomLevel).Add(key);
			}

			// Initialize the offline tile matrix.
			var centerKey = GetQuadKey(Map.Center, _zoomFloor, default(GoogleMapsLayer));
			var width = Map.ActualWidth;
			var height = Map.ActualHeight;

			// Calculate how many tiles are needed to cover the map horizontally.
			_tileMatrixLengthX = (int )(width / AbsoluteTileDimension) + 2;

			// Calculate how many tiles are needed to cover the map vertically.
			_tileMatrixLengthY = (int )(height / AbsoluteTileDimension) + 2;

			// Create the logical tile matrix.
			_tileMatrix = new Pushpin[_tileMatrixLengthX, _tileMatrixLengthY];

			// Calculate X index and Y index in the matrix of the center tile.
			var centerX = _tileMatrixLengthX / 2;
			var centerY = _tileMatrixLengthY / 2;

			// Create tile pushpins to fill the matrix.
			for (var x = 0; x < _tileMatrixLengthX; x++)
			{
				for (var y = 0; y < _tileMatrixLengthY; y++)
				{
					// Create the tile pushpin.
					var tile = new Pushpin
					{
						PositionOrigin = PositionOrigin.TopLeft,
						Style = _tilePushpinStyle,
						Background = new ImageBrush
						{
							ImageSource = _tileEmptyBackground,
							Stretch = Stretch.None
						}
					};

					// Add the tile to the matrix.
					_tileMatrix[x, y] = tile;

					// Calculate the differences between this tile's X/Y index and the center tile's X/Y index.
					var diffX = x - centerX;
					var diffY = y - centerY;

					// Calculate the quad key for this current tile.

					var key = GetNewKey(centerKey, diffX, diffY);

					// Update this tile with the calculated quad key.
					UpdateTile(tile, key);

					// Add this tile to the visual tree.
					OfflineStreetLayer.Children.Add(tile);
				}
			}

			_isOfflineModeInitialized = true;
		}

		/// <summary>
		/// Performs necessary actions start reacting to the offline tiles.
		/// </summary>
		private void EnterOfflineMode()
		{
			// Unhook event listeners to prevent duplicate event listening.
			_context.Center.ValueChanged -= OnCenterChanged;
			_context.ZoomLevel.ValueChanged -= OnZoomLevelChanged;
			_zoomTimer.Tick -= OnZoomTimerTick;

			// Listen to any map movements, and performs necessary re-arrangement for the offline tiles.
			_context.Center.ValueChanged += OnCenterChanged;

			// Listen to zome level changes and update the scaling ratio of all offline tiles.
			_context.ZoomLevel.ValueChanged += OnZoomLevelChanged;

			// Listen to zoom timer tick for re-arranging the offline tiles.
			_zoomTimer.Tick += OnZoomTimerTick;
		}

		/// <summary>
		/// Performs necessary actions to stop reacting to the offline tiles.
		/// </summary>
		private void LeaveOfflineMode()
		{
			_zoomTimer.Stop();
			_context.Center.ValueChanged -= OnCenterChanged;
			_context.ZoomLevel.ValueChanged -= OnZoomLevelChanged;
			_zoomTimer.Tick -= OnZoomTimerTick;
		}

		/// <summary>
		/// Calculates a new quad key based on the location, zoom level, and layer.
		/// </summary>
		private QuadKey GetQuadKey(GeoCoordinate location, int zoomLevel, GoogleMapsLayer layer)
		{
			var tileDimension = AbsoluteTileDimension * _tileScale;

			var tileCount = GetTileCount(zoomLevel);
			var quadMapDimension = tileDimension * tileCount / 2;
			var geoCenterPoint = Map.LocationToViewportPoint(_centerGeoCoordinate);
			var geoCenterPointX = geoCenterPoint.X;
			var geoCenterPointY = geoCenterPoint.Y;
			var currentPoint = Map.LocationToViewportPoint(location);
			var currentPointX = currentPoint.X;
			var currentPointY = currentPoint.Y;
			var mapEdgeToScreenEdgeX = quadMapDimension - geoCenterPointX;
			var mapEdgeToScreenEdgeY = quadMapDimension - geoCenterPointY;
			var mapEdgeToCurrentPointX = mapEdgeToScreenEdgeX + currentPointX;
			var mapEdgeToCurrentPointY = mapEdgeToScreenEdgeY + currentPointY;

			var x = mapEdgeToCurrentPointX / tileDimension;
			var y = mapEdgeToCurrentPointY / tileDimension;
			return new QuadKey((int )x, (int )y, zoomLevel, layer);
		}

		/// <summary>
		/// Gets the total number of horizontal/vertical tiles at the specified zoom level.
		/// </summary>
		private int GetTileCount(int zoomLevel)
		{
			return (int )Math.Pow(2D, zoomLevel);
		}

		/// <summary>
		/// Determines whether the specified <see cref="QuadKey"/> is within the valid range.
		/// </summary>
		private bool IsValidKey(QuadKey key)
		{
			var x = key.X;
			var y = key.Y;
			if (x < 0 || y < 0)
			{
				return false;
			}
			var tileCount = GetTileCount(key.ZoomLevel);
			return x < tileCount && y < tileCount;
		}

		/// <summary>
		/// Calculates a new <see cref="Point"/> represents where the specified <see cref="QuadKey"/> is in the map.
		/// </summary>
		private Point GetViewPortPoint(QuadKey key)
		{
			var x = key.X;
			var y = key.Y;
			var zoomLevel = key.ZoomLevel;
			var tileDimension = AbsoluteTileDimension * _tileScale;
			var tileCount = GetTileCount(zoomLevel);
			var tileNumberToGeoCenter = tileCount / 2;
			var modifierX = x >= tileNumberToGeoCenter ? 1 : -1;
			var modifierY = y >= tileNumberToGeoCenter ? 1 : -1;
			var xNumberToGeoCenter = Math.Abs(x - tileNumberToGeoCenter);
			var yNumberToGeoCenter = Math.Abs(y - tileNumberToGeoCenter);
			var geoCenterPoint = Map.LocationToViewportPoint(_centerGeoCoordinate);
			var geoCenterPointX = geoCenterPoint.X;
			var geoCenterPointY = geoCenterPoint.Y;

			var tilePointX = geoCenterPointX + (tileDimension * xNumberToGeoCenter * modifierX);
			var tilePointY = geoCenterPointY + (tileDimension * yNumberToGeoCenter * modifierY);
			var tilePoint = new Point(tilePointX, tilePointY);
			return tilePoint;
		}

		/// <summary>
		/// Lazy loads the cached keys list.
		/// </summary>
		private List<QuadKey> GetCachedKeys(int zoomLevel)
		{
			if (!_cachedTileImageKeys.ContainsKey(zoomLevel))
			{
				_cachedTileImageKeys[zoomLevel] = new List<QuadKey>();
			}
			return _cachedTileImageKeys[zoomLevel];
		}

		/// <summary>
		/// Gets the image where the specified <see cref="QuadKey"/> is representing.
		/// </summary>
		private void GetImage(QuadKey key, Action<Stream> callback)
		{
			var cachedKeys = GetCachedKeys(key.ZoomLevel);

			// Load the image from storage if it is already cached.
			if (cachedKeys.Contains(key))
			{
				var file = string.Concat(OfflineTileImagePrefix, key.Key);
				try
				{
					var stream = _fileStore.OpenFile(file, FileMode.Open);
					callback(stream);
				}
				catch
				{
					try
					{
						_fileStore.DeleteFile(file);
						callback(null);
					}
					catch
					{
						callback(null);
					}
				}
				return;
			}

			// Download and store the image.
			try
			{
				var uri = GoogleMapsTileSource.GetUri(key);
				var request = WebRequest.Create(uri);
				request.BeginGetResponse(result =>
				{
					try
					{
						var file = string.Concat(OfflineTileImagePrefix, key.Key);
						using (var response = request.EndGetResponse(result))
						{
							var imageStream = response.GetResponseStream();

							using (var fileStream = _fileStore.CreateFile(file))
							{
								var bytes = new byte[imageStream.Length];
								imageStream.Read(bytes, 0, bytes.Length);
								try
								{
									fileStream.Write(bytes, 0, bytes.Length);
								}
								catch
								{
									_fileStore.DeleteFile(file);
									throw;
								}
								cachedKeys.Add(key);
							}
							UIThread.InvokeAsync(() => callback(imageStream));
						}
					}
					catch
					{
						UIThread.InvokeAsync(() => callback(null));
					}
				}, null);
			}
			catch
			{
				callback(null);
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from top to bottom.
		/// </summary>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixTopToBottom(bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var x = 0; x <= maxIndexX; x++)
			{
				var temp = _tileMatrix[x, 0];
				for (var y = 0; y < maxIndexY; y++)
				{
					_tileMatrix[x, y] = _tileMatrix[x, y + 1];
				}
				_tileMatrix[x, maxIndexY] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey )temp.Tag, 0, _tileMatrixLengthY);

				// Update geo-coordinate and tile image of the moved tile.
				UpdateTile(temp, newKey, x > 0 || x < maxIndexY || (x == 0 && updateFirstTileImage) || (x == maxIndexX && updateLastTileImage));
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from left to right.
		/// </summary>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixLeftToRight(bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var y = 0; y <= maxIndexY; y++)
			{
				var temp = _tileMatrix[0, y];
				for (var x = 0; x < maxIndexX; x++)
				{
					_tileMatrix[x, y] = _tileMatrix[x + 1, y];
				}
				_tileMatrix[maxIndexX, y] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey )temp.Tag, _tileMatrixLengthX, 0);

				// Update geo-coordinate and tile image of the moved tile.
				UpdateTile(temp, newKey, y > 0 || y < maxIndexY || (y == 0 && updateFirstTileImage) || (y == maxIndexY && updateLastTileImage));
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from right to left.
		/// </summary>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixRightToLeft(bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var y = 0; y <= maxIndexY; y++)
			{
				var temp = _tileMatrix[maxIndexX, y];
				for (var x = maxIndexX; x > 0; x--)
				{
					_tileMatrix[x, y] = _tileMatrix[x - 1, y];
				}
				_tileMatrix[0, y] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey )temp.Tag, -_tileMatrixLengthX, 0);

				// Update geo-coordinate and tile image of the moved tile.
				UpdateTile(temp, newKey, y > 0 || y < maxIndexY || (y == 0 && updateFirstTileImage) || (y == maxIndexY && updateLastTileImage));
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from bottom to top.
		/// </summary>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixBottomToTop(bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var x = 0; x <= maxIndexX; x++)
			{
				var temp = _tileMatrix[x, maxIndexY];
				for (var y = maxIndexY; y > 0; y--)
				{
					_tileMatrix[x, y] = _tileMatrix[x, y - 1];
				}
				_tileMatrix[x, 0] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey )temp.Tag, 0, -_tileMatrixLengthY);

				// Update geo-coordinate and tile image of the moved tile.
				UpdateTile(temp, newKey, x > 0 || x < maxIndexY || (x == 0 && updateFirstTileImage) || (x == maxIndexX && updateLastTileImage));
			}
		}

		/// <summary>
		/// Calculates a new <see cref="QuadKey"/> based on the old key and the differences between the X count and Y count.
		/// </summary>
		private QuadKey GetNewKey(QuadKey oldKey, int diffX, int diffY)
		{
			var x = oldKey.X;
			var y = oldKey.Y;
			var zoomLevel = oldKey.ZoomLevel;
			var tileCount = GetTileCount(zoomLevel);

			var newX = x + diffX;
			if (newX < 0)
			{
				newX = tileCount + x + diffX;
			}
			else if (newX >= tileCount)
			{
				newX = diffX - (tileCount - x);
			}

			var newY = y + diffY;

			return new QuadKey(newX, newY, zoomLevel, oldKey.Layer);
		}

		/// <summary>
		/// Determines whether the specified pushpin is completely inside the map's view port.
		/// </summary>
		private bool IsInView(Pushpin tile)
		{
			var point = Map.LocationToViewportPoint(tile.Location);
			return IsVerticalInView(point) && IsHorizontalInView(point);
		}

		/// <summary>
		/// Determines whether the top and bottom edges of the specified pushpin are inside the map's view port.
		/// </summary>
		private bool IsVerticalInView(Pushpin tile)
		{
			var point = Map.LocationToViewportPoint(tile.Location);
			return IsVerticalInView(point);
		}

		/// <summary>
		/// Determines whether the top and bottom edges of the tile at the specified position are inside the map's view port.
		/// </summary>
		private bool IsVerticalInView(Point tilePosition)
		{
			var top = tilePosition.Y;
			var bottom = top + AbsoluteTileDimension * _tileScale;
			return top >= 0 && bottom <= Map.ActualHeight;
		}

		/// <summary>
		/// Determines whether the left and right edges of the specified pushpin are inside the map's view port.
		/// </summary>
		private bool IsHorizontalInView(Pushpin tile)
		{
			var point = Map.LocationToViewportPoint(tile.Location);
			return IsHorizontalInView(point);
		}

		/// <summary>
		/// Determines whether the left and right edges of the tile at the specified position are inside the map's view port.
		/// </summary>
		private bool IsHorizontalInView(Point tilePosition)
		{
			var left = tilePosition.X;
			var right = tilePosition.X + AbsoluteTileDimension * _tileScale;
			return left >= 0 && right <= Map.ActualWidth;
		}

		/// <summary>
		/// Updates the geo-location and optionally the image of the specified tile based on the given key.
		/// </summary>
		private void UpdateTile(Pushpin tile, QuadKey key, bool updateImage = true)
		{
			tile.Tag = key;
			var brush = ((ImageBrush )tile.Background);
			brush.ImageSource = _tileEmptyBackground;
			if (!IsValidKey(key))
			{
				return;
			}
			var newPoint = GetViewPortPoint(key);
			var newLocation = Map.ViewportPointToLocation(newPoint);
			tile.Location = newLocation;
			if (!updateImage)
			{
				return;
			}
			GetImage(key, stream =>
			{
				// Only continue updating the image source if the key attached to the tile is still the same key.
				if ((QuadKey )tile.Tag != key)
				{
					return;
				}

				if (stream == null)
				{
					return;
				}
				var source = (BitmapImage)brush.ImageSource;
				if (source == _tileEmptyBackground)
				{
					brush.ImageSource = source = new BitmapImage();
				}
				source.SetSource(stream);
			});
		}

		/// <summary>
		/// Updates the tile scaling ratio according to the current map zoom level.
		/// </summary>
		private void UpdateTileScale()
		{
			_tileScaleTransform.ScaleX = _tileScaleTransform.ScaleY = _tileScale = Math.Pow(2D, (Map.ZoomLevel - _zoomFloor));
		}

		#endregion
	}
}
