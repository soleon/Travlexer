﻿using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using Travlexer.WindowsPhone.Core.Extensions;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
	public partial class MapView
	{
		private readonly MapViewModel _context;

		public MapView()
		{
			InitializeComponent();
			_context = DataContext as MapViewModel;
			if (_context != null)
			{
				_context.MapBoundCapturer = OnCaptureMapBound;
				_context.SelectedPushpinChanged += OnSelectedPushpinChanged;
			}

			// The "Hold" event in XAML is not recognised by Blend.
			// Doing event handling here instead of in XAML is a hack to make the view still "blendable".
			Map.Hold += OnMapHold;
		}


		#region Event Handling

		/// <summary>
		/// Called when the selected pushpin of the data context is changed.
		/// </summary>
		/// <param name="vm">The vm.</param>
		private void OnSelectedPushpinChanged(PushpinViewModel vm)
		{
			VisualStateManager.GoToState(this, vm == null ? "Default" : "PushpinSelected", true);
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

		#endregion
	}
}
