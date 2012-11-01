using System;
using System.Windows;
using System.Windows.Media;
using Codify.Extensions;
using Microsoft.Phone.Controls.Maps;

namespace Travlexer.WindowsPhone.Controls
{
    public class MapPolyline : Microsoft.Phone.Controls.Maps.MapPolyline
    {
        private bool _isLoaded, _isUsingReduction;

        public MapPolyline()
        {
            Loaded += (s, e) =>
            {
                _isLoaded = true;
                if (_isUsingReduction) UpdateLocations();
            };
            Unloaded += (s, e) => _isLoaded = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (ParentMap == null) return;
            Clip = new RectangleGeometry {Rect = new Rect(0, 0, ParentMap.ViewportSize.Width, ParentMap.ViewportSize.Height)};
        }


        #region Properties


        #region Stroke

        public new Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        ///   Defines the <see cref="Stroke" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke",
            typeof (Brush),
            typeof (MapPolyline),
            new PropertyMetadata(default(Brush), (s, t) => ((MapPolyline) s).OnStrokeChanged((Brush) t.NewValue))
            );

        /// <summary>
        ///   Called when <see cref="Stroke" /> changes.
        /// </summary>
        private void OnStrokeChanged(Brush newValue)
        {
            base.Stroke = newValue;
        }

        #endregion


        #region ZoomLevel

        public double ZoomLevel
        {
            get { return (double) GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        /// <summary>
        ///   Defines the <see cref="ZoomLevel" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register(
            "ZoomLevel",
            typeof (double),
            typeof (MapPolyline),
            new PropertyMetadata(double.MinValue, (s, e) => ((MapPolyline) s).OnZoomLevelChanged()));

        /// <summary>
        ///   Called when <see cref="ZoomLevel" /> changes.
        /// </summary>
        private void OnZoomLevelChanged()
        {
            if (!_isUsingReduction || Locations.IsNullOrEmpty()) return;
            UpdateLocations();
        }

        #endregion


        #region Locations

        public new LocationCollection Locations
        {
            get { return (LocationCollection) GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        /// <summary>
        ///   Defines the <see cref="Locations" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocationsProperty = DependencyProperty.Register(
            "Locations",
            typeof (LocationCollection),
            typeof (MapPolyline),
            new PropertyMetadata(default(LocationCollection), (s, e) => ((MapPolyline) s).OnLocationsChanged((LocationCollection) e.NewValue)));

        /// <summary>
        ///   Called when <see cref="Locations" /> changes.
        /// </summary>
        private void OnLocationsChanged(LocationCollection newValue)
        {
            _isUsingReduction = newValue != null && newValue.Count > 3000;
            if (_isUsingReduction)
            {
                if (ZoomLevel.Equals(double.MinValue)) return;
                UpdateLocations();
            }
            else base.Locations = newValue;
        }

        #endregion


        #endregion


        private void UpdateLocations()
        {
            if (!_isLoaded || ZoomLevel.Equals(double.MinValue) || Locations.IsNullOrEmpty()) return;
            var tolorance = Math.Pow(1/ZoomLevel, 3D);
            base.Locations = Utility.DouglasPeuckerReduction(Locations, tolorance);
        }
    }
}