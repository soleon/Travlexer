using System;
using System.Windows;

namespace Codify.WindowsPhone.Triggers
{
	public class EventTrigger : System.Windows.Interactivity.EventTrigger
	{
		#region Public Properties

		public bool CanTrigger
		{
			get { return (bool) GetValue(CanTriggerProperty); }
			set { SetValue(CanTriggerProperty, value); }
		}

		public static readonly DependencyProperty CanTriggerProperty = DependencyProperty.Register(
			"CanTrigger",
			typeof (bool),
			typeof (EventTrigger),
			new PropertyMetadata(true));

		public bool IsHandled
		{
			get { return (bool) GetValue(IsHandledProperty); }
			set { SetValue(IsHandledProperty, value); }
		}

		public static readonly DependencyProperty IsHandledProperty = DependencyProperty.Register(
			"IsHandled",
			typeof (bool),
			typeof (EventTrigger),
			null);

		#endregion


		#region Protected Methods

		/// <summary>
		/// Checks <see cref="CanTrigger"/> before firing the event associated with this EventTrigger.
		/// </summary>
		protected override void OnEvent(EventArgs eventArgs)
		{
			var handledProperty = eventArgs.GetType().GetProperty("Handled");
			if (handledProperty != null)
			{
				handledProperty.SetValue(eventArgs, IsHandled, null);
			}
			if (CanTrigger)
			{
				base.OnEvent(eventArgs);
			}
		}

		#endregion
	}
}
