using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Travlexer.WindowsPhone.Controls
{
    public class CurrentLocationIndicator : ContentControl
    {
        private readonly ControlTemplate _templateIconCurrentLocation;
        private readonly ControlTemplate _templateIconHeadedCurrentLocation;

        public CurrentLocationIndicator()
        {
            RenderTransformOrigin = new Point(0.5, 0.6);
            Template = _templateIconCurrentLocation = (ControlTemplate) Application.Current.Resources["IconCurrentLocation"];
            _templateIconHeadedCurrentLocation = (ControlTemplate) Application.Current.Resources["IconHeadedCurrentLocation"];
        }


        #region HasHeading

        public bool HasHeading
        {
            get { return (bool) GetValue(HasHeadingProperty); }
            set { SetValue(HasHeadingProperty, value); }
        }

        /// <summary>
        ///     Defines the <see cref="HasHeading" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasHeadingProperty = DependencyProperty.Register(
            "HasHeading",
            typeof (bool),
            typeof (CurrentLocationIndicator),
            new PropertyMetadata(default(bool), (s, e) => ((CurrentLocationIndicator) s).OnHasHeadingChanged((bool) e.NewValue)));

        /// <summary>
        ///     Called when <see cref="HasHeading" /> changes.
        /// </summary>
        private void OnHasHeadingChanged(bool newValue)
        {
            Template = newValue ? _templateIconHeadedCurrentLocation : _templateIconCurrentLocation;
        }

        #endregion


        #region Heading

        public double Heading
        {
            get { return (double) GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        /// <summary>
        ///     Defines the <see cref="Heading" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
            "Heading",
            typeof (double),
            typeof (CurrentLocationIndicator),
            new PropertyMetadata(default(double), (s, e) => ((CurrentLocationIndicator) s).OnHeadingChanged((double) e.NewValue)));

        /// <summary>
        ///     Called when <see cref="Heading" /> changes.
        /// </summary>
        private void OnHeadingChanged(double newValue)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (!(RenderTransform is RotateTransform)) RenderTransform = new RotateTransform {Angle = newValue};
                else ((RotateTransform) RenderTransform).Angle = newValue;
            });
        }

        #endregion
    }
}