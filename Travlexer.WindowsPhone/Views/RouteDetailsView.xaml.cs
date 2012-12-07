using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Codify.Attributes;
using Microsoft.Phone.Controls.Maps;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof (RouteDetailsViewModel))]
    public partial class RouteDetailsView
    {
        private const double CloseUpZoomLevel = 18D;

        public RouteDetailsView()
        {
            InitializeComponent();
            SetBinding(MapViewLocationsProperty, new Binding(RouteDetailsViewModel.MapviewLocationsProperty));
            Loaded += (s, e) => Map.AnimationLevel = ApplicationContext.Data.UseMapAnimation.Value ? AnimationLevel.Full : AnimationLevel.UserInput;
        }


        #region MapViewLocations

        public IEnumerable<Location> MapViewLocations
        {
            get { return (IEnumerable<Location>) GetValue(MapViewLocationsProperty); }
            set { SetValue(MapViewLocationsProperty, value); }
        }

        /// <summary>
        ///     Defines the <see cref="MapViewLocations" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapViewLocationsProperty = DependencyProperty.Register(
            "MapViewLocations",
            typeof (IEnumerable<Location>),
            typeof (RouteDetailsView),
            new PropertyMetadata(default(IEnumerable<Location>), (s, e) => ((RouteDetailsView) s).OnMapViewLocationsChanged((IEnumerable<Location>) e.NewValue)));

        /// <summary>
        ///     Called when <see cref="MapViewLocations" /> changes.
        /// </summary>
        private void OnMapViewLocationsChanged(IEnumerable<Location> newValue)
        {
            var geoCoordinates = newValue.Select(l => l.ToGeoCoordinate()).ToArray();
            if (geoCoordinates.Length == 1)
            {
                Map.Center = geoCoordinates[0];
                Map.ZoomLevel = CloseUpZoomLevel;
            }
            else Map.SetView(LocationRect.CreateLocationRect(geoCoordinates));
        }

        #endregion


        private void GoToStepButtonClick(object sender, RoutedEventArgs e)
        {
            Pivot.SelectedItem = MapPivotItem;
        }
    }
}