using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Travlexer.WindowsPhone.Core.Triggers
{
	/// <summary>
	/// Executes a collection of action when a particular key is pressed.
	/// </summary>
	[Description("Executes a collection of action when a particular key is pressed.")]
	public class KeyPressTrigger : TriggerBase<FrameworkElement>
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the key to trigger the actions when it is pressed.
		/// </summary>
		public Key Key
		{
			get { return (Key) GetValue(KeyProperty); }
			set { SetValue(KeyProperty, value); }
		}

		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
			"Key",
			typeof (Key),
			typeof (KeyPressTrigger),
			null);

		/// <summary>
		/// Gets or sets a value indicating whether this trigger also handles the key press event when the specified <see cref="Key"/> is pressed.
		/// </summary>
		public bool IsKeyPressHandled
		{
			get { return (bool)GetValue(IsKeyPressHandledProperty); }
			set { SetValue(IsKeyPressHandledProperty, value); }
		}

		public static readonly DependencyProperty IsKeyPressHandledProperty = DependencyProperty.Register(
			"IsKeyPressHandled",
			typeof(bool),
			typeof(KeyPressTrigger),
			new PropertyMetadata(true));

		#endregion


		#region Event Handling

		/// <summary>
		/// Called after the trigger is attached to an AssociatedObject.
		/// </summary>
		protected override void OnAttached()
		{
			AssociatedObject.KeyUp += OnKeyUp;
			base.OnAttached();
		}

		/// <summary>
		/// Called when the trigger is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		protected override void OnDetaching()
		{
			AssociatedObject.KeyUp -= OnKeyUp;
			base.OnDetaching();
		}

		/// <summary>
		/// Called when KeyUp event is raised on the <see cref="TriggerBase{T}.AssociatedObject"/>.
		/// </summary>
		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key != Key)
			{
				return;
			}
			if (IsKeyPressHandled)
			{
				e.Handled = true;
			}
			InvokeActions(null);
		}

		#endregion
	}
}