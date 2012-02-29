using System.Windows;
using System.Windows.Media;

namespace Travlexer.WindowsPhone.Controls
{
	public class MapPolyline : Microsoft.Phone.Controls.Maps.MapPolyline
	{
		#region Stroke

		public new Brush Stroke
		{
			get { return (Brush) GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		/// <summary>
		/// Defines the <see cref="Stroke"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
			"Stroke",
			typeof (Brush),
			typeof (MapPolyline),
			new PropertyMetadata(default(Brush), (s, t) => ((MapPolyline)s).OnStrokeChanged((Brush) t.NewValue))
			);

		/// <summary>
		/// Called when <see cref="Stroke"/> changes.
		/// </summary>
		private void OnStrokeChanged(Brush newValue)
		{
			base.Stroke = newValue;
		}

		#endregion
	}
}