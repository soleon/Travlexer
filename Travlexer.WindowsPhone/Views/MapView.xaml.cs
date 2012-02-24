using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Codify.Models;
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
			MapTileScaleName = "TileScale";

		private const int AbsoluteTileDimension = 256;

		#endregion


		#region Private Members

		private readonly MapViewModel _context;
		private readonly IApplicationBar _appBar;
		private readonly Dictionary<int, List<QuadKey>> _cachedTileImageKeys = new Dictionary<int, List<QuadKey>>();
		private readonly Style _tilePushpinStyle;
		private readonly ScaleTransform _tileScaleTransform;

		private readonly IsolatedStorageFile _fileStore = IsolatedStorageFile.GetUserStoreForApplication();
		private readonly GeoCoordinate _centerGeoCoordinate = new GeoCoordinate(0D, 0D);
		private readonly DispatcherTimer _zoomTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200D) };
		private readonly Queue<KeyValuePair<QuadKey, Stream>> _tileImageCache = new Queue<KeyValuePair<QuadKey, Stream>>(50);

		private readonly ObservableValue<ExpansionStates> _toolbarState = Infrastructure.ApplicationContext.ToolbarState;
		private readonly ObservableValue<bool> _isOnline = Infrastructure.ApplicationContext.IsOnline;
		private readonly ObservableValue<GeoCoordinate> _mapCenter = Infrastructure.DataContext.MapCenter;
		private readonly ObservableValue<GoogleMapsLayer> _mapBase = Infrastructure.DataContext.MapBaseLayer;
		private readonly ObservableValue<double> _mapZoomLevel = Infrastructure.DataContext.MapZoomLevel;
		private readonly ObservableCollection<GoogleMapsLayer> _mapOverlays = Infrastructure.DataContext.MapOverlays;

		private bool _isZooming;
		private double _tileScale;
		private int _zoomFloor, _tileMatrixLengthX, _tileMatrixLengthY;
		private GoogleMapsLayer _currentLayer;
		private Pushpin[,] _baseTileMatrix, _transitTileMatrix;

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
			((Grid)Content).Children.Add(_debugText = new TextBlock { IsHitTestVisible = false, RenderTransform = new TranslateTransform { Y = 30 }, Foreground = new SolidColorBrush(Colors.Red), Text = "debug" });
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
			_toolbarState.ValueChanged += (old, @new) => GoToToolbarState(@new);
			_isOnline.ValueChanged += OnIsOnlineValueChanged;

			GoToVisualState(_context.VisualState.Value, false);
			GoToToolbarState(_toolbarState.Value, false);

			_tilePushpinStyle = (Style)Resources[MapTilePushpinStyleName];
			_tileScaleTransform = (ScaleTransform)Resources[MapTileScaleName];
			_tileScale = 1D;

			if (!_isOnline.Value)
			{
				Map.Loaded += (s, e) =>
				{
					InitializeOfflineLayer(ref _baseTileMatrix, OfflineBaseLayer, _mapBase.Value);
					if (OfflineBaseLayer.Visibility == Visibility.Visible)
					{
						InitializeOfflineLayer(ref _transitTileMatrix, OfflineTransitLayer, GoogleMapsLayer.TransitOverlay);
					}
					RegisterOfflineModeEventListeners();
				};
			}
		}

		private void OnIsOnlineValueChanged(bool old, bool @new)
		{
			if (@new)
			{
				UnregisterOfflineModeEventListeners();
			}
			else
			{
				// Check base layer.
				if (OfflineBaseLayer.Tag == null)
				{
					InitializeOfflineLayer(ref _baseTileMatrix, OfflineBaseLayer, _mapBase.Value);
				}
				else
				{
					RefreshOfflineTiles(_baseTileMatrix, _mapBase.Value, false);
				}

				// Check transit layer.
				if (OfflineBaseLayer.Visibility == Visibility.Visible)
				{
					if (OfflineTransitLayer.Tag == null)
					{
						InitializeOfflineLayer(ref _transitTileMatrix, OfflineTransitLayer, GoogleMapsLayer.TransitOverlay);
					}
					else
					{
						RefreshOfflineTiles(_transitTileMatrix, GoogleMapsLayer.TransitOverlay, false);
					}
				}

				// Register necessary event listeners.
				RegisterOfflineModeEventListeners();
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
			_mapCenter.Value = dragPushpinVm.Data.Location = location;

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

		/// <summary>
		/// Called when the zoom timer ticks. This re-arranges all offline tiles according to the current center point and zoom level.
		/// </summary>
		private void OnZoomTimerTick(object sender, EventArgs eventArgs)
		{
			_zoomTimer.Stop();
			_isZooming = false;
			RefreshOfflineTiles(_baseTileMatrix, _mapBase.Value);
			if (OfflineTransitLayer.Visibility == Visibility.Visible)
			{
				RefreshOfflineTiles(_transitTileMatrix, GoogleMapsLayer.TransitOverlay);
			}
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

			RearrangeMatrix(_baseTileMatrix);
			if (OfflineTransitLayer.Visibility == Visibility.Visible)
			{
				RearrangeMatrix(_transitTileMatrix);
			}
		}

		/// <summary>
		/// Called when the value of <see cref="Infrastructure.DataContext.MapBaseLayer"/> has changed.
		/// </summary>
		private void OnMapBaseLayerValueChanged(GoogleMapsLayer old, GoogleMapsLayer @new)
		{
			RefreshOfflineTiles(_baseTileMatrix, @new, false);
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
				case MapViewModel.VisualStates.Route:
					_appBar.IsVisible = false;
					if (FromTextBox.Text.Length == 0)
					{
						FromTextBox.Focus();
					}
					else if (ToTextBox.Text.Length == 0)
					{
						ToTextBox.Focus();
					}
					else
					{
						FromTextBox.Focus();
					}
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
		private void InitializeOfflineLayer(ref Pushpin[,] matrix, Panel mapLayer, GoogleMapsLayer layer)
		{
			// Check if this layer has already been initialised.
			if (mapLayer.Tag != null)
			{
				return;
			}

			// Get the zoom level floor from the current map zoom level.
			_zoomFloor = (int)Map.ZoomLevel;

			_currentLayer = _mapBase.Value;

			// Update the tile scaling ratio according to the current map zoom level.
			UpdateTileScale();

			// Construct the in-memory list of quad keys that represents the cached tile images.
			var keys = _fileStore.GetFileNames(OfflineTilesSearchPattern).Select(f => new QuadKey(f.Substring(OfflineTileImagePrefix.Length)));
			foreach (var key in keys)
			{
				GetCachedKeys(key.ZoomLevel).Add(key);
			}

			// Initialize the offline tile matrix.
			var centerKey = GetQuadKey(Map.Center, _zoomFloor, layer);
			var width = Map.ActualWidth;
			var height = Map.ActualHeight;

			// Calculate how many tiles are needed to cover the map horizontally.
			_tileMatrixLengthX = (int)(width / AbsoluteTileDimension) + 2;

			// Calculate how many tiles are needed to cover the map vertically.
			_tileMatrixLengthY = (int)(height / AbsoluteTileDimension) + 2;

			// Create the logical tile matrix.
			matrix = new Pushpin[_tileMatrixLengthX, _tileMatrixLengthY];

			// Calculate X index and Y index in the matrix of the center tile.
			var centerX = _tileMatrixLengthX / 2;
			var centerY = _tileMatrixLengthY / 2;

			// Create tile pushpins to fill the matrix.
			for (var x = 0; x < _tileMatrixLengthX; x++)
			{
				for (var y = 0; y < _tileMatrixLengthY; y++)
				{
					// Create tile pushpin.
					var tile = new Pushpin
					{
						PositionOrigin = PositionOrigin.TopLeft,
						Style = _tilePushpinStyle,
						Background = new ImageBrush
						{
							Stretch = Stretch.None
						}
					};

					// Add the tile to the matrix.
					matrix[x, y] = tile;

					// Calculate the differences between this tile's X/Y index and the center tile's X/Y index.
					var diffX = x - centerX;
					var diffY = y - centerY;

					// Calculate the quad key for this current tile.
					var key = GetNewKey(centerKey, diffX, diffY);

					// Update this tile with the calculated quad key.
					UpdateTile(tile, key);

					// Add this tile to the visual tree.
					mapLayer.Children.Add(tile);
				}
			}

			// Flag that this layer is now initialised.
			mapLayer.Tag = true;
		}

		/// <summary>
		/// Listens to all necessary events for the offline mode.
		/// </summary>
		private void RegisterOfflineModeEventListeners()
		{
			// Unregister event listeners to prevent duplicate event listening.
			UnregisterOfflineModeEventListeners();

			// Listen to any map movements, and performs necessary re-arrangement for the offline tiles.
			_mapCenter.ValueChanged += OnCenterChanged;

			// Listen to zome level changes and update the scaling ratio of all offline tiles.
			_mapZoomLevel.ValueChanged += OnZoomLevelChanged;

			// Listen to zoom timer tick for re-arranging the offline tiles.
			_zoomTimer.Tick += OnZoomTimerTick;

			// Listen to map base layer change and refersh offline tile images.
			_mapBase.ValueChanged += OnMapBaseLayerValueChanged;

			// Listen to map overlay changes and show/hide the transit layer.
			_mapOverlays.CollectionChanged += OnMapOverlaysCollectionChanged;
		}

		/// <summary>
		/// Called when items in <see cref="Infrastructure.DataContext.MapOverlays"/> has changed.
		/// This handler specifically checks if <see cref="GoogleMapsLayer.TransitOverlay"/> is added to the collection, and refershes the offline transit layer if the map is in offline mode.
		/// </summary>
		private void OnMapOverlaysCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (OfflineTransitLayer.Visibility == Visibility.Visible)
			{
				RefreshOfflineTiles(_transitTileMatrix, GoogleMapsLayer.TransitOverlay, false);
			}
		}

		/// <summary>
		/// Performs necessary actions to stop reacting to the offline tiles.
		/// </summary>
		private void UnregisterOfflineModeEventListeners()
		{
			_zoomTimer.Stop();
			_mapCenter.ValueChanged -= OnCenterChanged;
			_mapZoomLevel.ValueChanged -= OnZoomLevelChanged;
			_zoomTimer.Tick -= OnZoomTimerTick;
			_mapBase.ValueChanged -= OnMapBaseLayerValueChanged;
			_isZooming = false;
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
			return new QuadKey((int)x, (int)y, zoomLevel, layer);
		}

		/// <summary>
		/// Gets the total number of horizontal/vertical tiles at the specified zoom level.
		/// </summary>
		private int GetTileCount(int zoomLevel)
		{
			return (int)Math.Pow(2D, zoomLevel);
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
				foreach (var cache in _tileImageCache.Where(cache => cache.Key == key))
				{
					callback(cache.Value);
					return;
				}
				var file = string.Concat(OfflineTileImagePrefix, key.Key);
				Stream stream = null;
				try
				{
					stream = _fileStore.OpenFile(file, FileMode.Open);
					_tileImageCache.Enqueue(new KeyValuePair<QuadKey, Stream>(key, stream));
				}
				catch { }
				finally
				{
					callback(stream);
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
					Stream stream = null;
					try
					{
						var file = OfflineTileImagePrefix + key.Key;
						using (var response = request.EndGetResponse(result))
						{
							stream = response.GetResponseStream();

							using (var fileStream = _fileStore.CreateFile(file))
							{
								var bytes = new byte[stream.Length];
								stream.Read(bytes, 0, bytes.Length);
								fileStream.Write(bytes, 0, bytes.Length);
								cachedKeys.Add(key);
								_tileImageCache.Enqueue(new KeyValuePair<QuadKey, Stream>(key, stream));
							}
						}
					}
					catch { }
					finally
					{
						UIThread.InvokeAsync(() => callback(stream));
					}
				}, null);
			}
			catch
			{
				callback(null);
			}
		}

		/// <summary>
		/// Rearranges the target tile matrix according to which edge tile has been moved into view.
		/// </summary>
		private void RearrangeMatrix(Pushpin[,] matrix)
		{
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
			var tile = matrix[0, 0];
			var key = (QuadKey)tile.Tag;
			var y = key.Y;
			if (IsInView(tile))
			{
				RearrangeMatrixRightToLeft(matrix, updateLastTileImage: y == 0);
				if (y > 0)
				{
					RearrangeMatrixBottomToTop(matrix);
				}
				return;
			}

			// Top right:
			tile = matrix[maxIndexX, 0];
			key = (QuadKey)tile.Tag;
			y = key.Y;
			if (IsInView(_baseTileMatrix[maxIndexX, 0]))
			{
				RearrangeMatrixLeftToRight(matrix, updateLastTileImage: y == 0);
				if (y > 0)
				{
					RearrangeMatrixBottomToTop(matrix);
				}
				return;
			}

			// Bottom left:
			tile = matrix[0, maxIndexY];
			key = (QuadKey)tile.Tag;
			y = key.Y;
			if (IsInView(tile))
			{
				if (y < maxTileIndex)
				{
					RearrangeMatrixTopToBottom(matrix, updateLastTileImage: false);
				}
				RearrangeMatrixRightToLeft(matrix);
				return;
			}

			// Bottom right:
			tile = matrix[maxIndexX, maxIndexY];
			key = (QuadKey)tile.Tag;
			y = key.Y;
			if (IsInView(tile))
			{
				if (y < maxTileIndex)
				{
					RearrangeMatrixTopToBottom(matrix, false);
				}
				RearrangeMatrixLeftToRight(matrix);
				return;
			}

			// Top:
			tile = matrix[maxIndexX / 2 + maxIndexX % 2, 0];
			key = (QuadKey)tile.Tag;
			y = key.Y;
			if (y > 0 && IsVerticalInView(tile))
			{
				RearrangeMatrixBottomToTop(matrix);
				return;
			}

			// Bottom:
			tile = matrix[maxIndexX / 2 + maxIndexX % 2, maxIndexY];
			key = (QuadKey)tile.Tag;
			y = key.Y;
			if (y < maxTileIndex && IsVerticalInView(tile))
			{
				RearrangeMatrixTopToBottom(matrix);
				return;
			}

			// Left:
			tile = matrix[0, maxIndexY / 2 + maxIndexY % 2];
			if (IsHorizontalInView(tile))
			{
				RearrangeMatrixRightToLeft(matrix);
				return;
			}

			// Right:
			tile = matrix[maxIndexX, maxIndexY / 2 + maxIndexY % 2];
			if (IsHorizontalInView(tile))
			{
				RearrangeMatrixLeftToRight(matrix);
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from top to bottom.
		/// </summary>
		/// <param name="matrix">The target matrix to arrange the tiles.</param>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixTopToBottom(Pushpin[,] matrix, bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var x = 0; x <= maxIndexX; x++)
			{
				var temp = matrix[x, 0];
				for (var y = 0; y < maxIndexY; y++)
				{
					matrix[x, y] = matrix[x, y + 1];
				}
				matrix[x, maxIndexY] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey)temp.Tag, 0, _tileMatrixLengthY);

				// Update geo-coordinate and tile image of the moved tile.
				UpdateTile(temp, newKey, x > 0 || x < maxIndexY || (x == 0 && updateFirstTileImage) || (x == maxIndexX && updateLastTileImage));
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from left to right.
		/// </summary>
		/// <param name="matrix">The target matrix to arrange the tiles.</param>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixLeftToRight(Pushpin[,] matrix, bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var y = 0; y <= maxIndexY; y++)
			{
				var temp = matrix[0, y];
				for (var x = 0; x < maxIndexX; x++)
				{
					matrix[x, y] = matrix[x + 1, y];
				}
				matrix[maxIndexX, y] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey)temp.Tag, _tileMatrixLengthX, 0);

				// Update geo-coordinate and tile image of the moved tile.
				UpdateTile(temp, newKey, y > 0 || y < maxIndexY || (y == 0 && updateFirstTileImage) || (y == maxIndexY && updateLastTileImage));
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from right to left.
		/// </summary>
		/// <param name="matrix">The target matrix to arrange the tiles.</param>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixRightToLeft(Pushpin[,] matrix, bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var y = 0; y <= maxIndexY; y++)
			{
				var temp = matrix[maxIndexX, y];
				for (var x = maxIndexX; x > 0; x--)
				{
					matrix[x, y] = matrix[x - 1, y];
				}
				matrix[0, y] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey)temp.Tag, -_tileMatrixLengthX, 0);

				// Update geo-coordinate and tile image of the moved tile.
				UpdateTile(temp, newKey, y > 0 || y < maxIndexY || (y == 0 && updateFirstTileImage) || (y == maxIndexY && updateLastTileImage));
			}
		}

		/// <summary>
		/// Rearranges the offline tiles in the matrix from bottom to top.
		/// </summary>
		/// <param name="matrix">The target matrix to arrange the tiles.</param>
		/// <param name="updateFirstTileImage">if set to <c>false</c> ignore updating the image of the first tile.</param>
		/// <param name="updateLastTileImage">if set to <c>false</c> ignore updating the image of the last tile.</param>
		private void RearrangeMatrixBottomToTop(Pushpin[,] matrix, bool updateFirstTileImage = true, bool updateLastTileImage = true)
		{
			var maxIndexX = _tileMatrixLengthX - 1;
			var maxIndexY = _tileMatrixLengthY - 1;
			for (var x = 0; x <= maxIndexX; x++)
			{
				var temp = matrix[x, maxIndexY];
				for (var y = maxIndexY; y > 0; y--)
				{
					matrix[x, y] = matrix[x, y - 1];
				}
				matrix[x, 0] = temp;

				// Calculate the quad key of the new location.
				var newKey = GetNewKey((QuadKey)temp.Tag, 0, -_tileMatrixLengthY);

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
			var brush = ((ImageBrush)tile.Background);
			brush.ImageSource = null;
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
				if ((QuadKey)tile.Tag != key)
				{
					return;
				}

				if (stream == null)
				{
					return;
				}
				var source = (BitmapImage)brush.ImageSource;
				if (source == null)
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

		/// <summary>
		/// Rearranges all offline tiles and updates their image according to the current location, zoom level and layer.
		/// </summary>
		private void RefreshOfflineTiles(Pushpin[,] matrix, GoogleMapsLayer layer, bool similarityCheck = true)
		{
			if (_isZooming)
			{
				return;
			}

			var oldZoomFloor = _zoomFloor;
			_zoomFloor = (int)Map.ZoomLevel;

			var oldLayer = _currentLayer;
			_currentLayer = layer;

			if (similarityCheck && oldZoomFloor == _zoomFloor && oldLayer == _currentLayer)
			{
				return;
			}

			UpdateTileScale();

			// Update all tiles if the zoom level has reached a new floor.
			var centerKey = GetQuadKey(Map.Center, _zoomFloor, layer);

			// Calculate X index and Y index in the matrix of the center tile.
			var centerX = _tileMatrixLengthX / 2;
			var centerY = _tileMatrixLengthY / 2;

			// Update location and image of tile pushpins.
			for (var x = 0; x < _tileMatrixLengthX; x++)
			{
				for (var y = 0; y < _tileMatrixLengthY; y++)
				{
					// Create the tile pushpin.
					var tile = matrix[x, y];

					// Calculate the differences between this tile's X/Y index and the center tile's X/Y index.
					var diffX = x - centerX;
					var diffY = y - centerY;

					// Calculate the quad key for this current tile.
					var key = new QuadKey(centerKey.X + diffX, centerKey.Y + diffY, _zoomFloor, layer);

					// Update this tile with the calculated quad key.
					UpdateTile(tile, key);
				}
			}
		}

		/// <summary>
		/// Called when a <see cref="TextBox"/> gets its focus.
		/// Selects all texts of the <see cref="TextBox"/>.
		/// </summary>
		private void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
		{
			((TextBox)sender).SelectAll();
		}

		#endregion
	}
}
