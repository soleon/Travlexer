using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Travlexer.WindowsPhone.Core.Behaviors
{
	public class CancelGestureEventBehavior : Behavior<UIElement>
	{
		#region Public Members

		public enum Events : byte
		{
			All = 0,
			Tap,
			DoubleTap,
			Hold,
		}

		#endregion


		#region Event Handling

		protected override void OnAttached()
		{
			switch (Event)
			{
				case Events.Tap:
					AssociatedObject.Tap += OnGestureEvent;
					break;
				case Events.DoubleTap:
					AssociatedObject.DoubleTap += OnGestureEvent;
					break;
				case Events.Hold:
					AssociatedObject.Hold += OnGestureEvent;
					break;
				default:
					AssociatedObject.Tap += OnGestureEvent;
					AssociatedObject.DoubleTap += OnGestureEvent;
					AssociatedObject.Hold += OnGestureEvent;
					break;
			}
			base.OnAttached();
		}

		protected override void OnDetaching()
		{
			switch (Event)
			{
				case Events.Tap:
					AssociatedObject.Tap -= OnGestureEvent;
					break;
				case Events.DoubleTap:
					AssociatedObject.DoubleTap -= OnGestureEvent;
					break;
				case Events.Hold:
					AssociatedObject.Hold -= OnGestureEvent;
					break;
				default:
					AssociatedObject.Tap -= OnGestureEvent;
					AssociatedObject.DoubleTap -= OnGestureEvent;
					AssociatedObject.Hold -= OnGestureEvent;
					break;
			}
			base.OnDetaching();
		}

		private static void OnGestureEvent(object sender, GestureEventArgs e)
		{
			e.Handled = true;
		}

		#endregion


		#region Public Properties

		public Events Event { get; set; }

		#endregion
	}
}
