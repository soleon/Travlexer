using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Codify.WindowsPhone.Triggers
{
	/// <summary>
	/// Executes a collection of actions when the <see cref="Application.RootVisual"/> raises any <see cref="FrameworkElement.MouseLeftButtonDown"/> activity.
	/// </summary>
	[Description("Executes a collection of action when the Application.RootVisual raises any MouseLeftButtonDown activity.")]
	public class RootTouchTrigger : TriggerBase<FrameworkElement>
	{
		#region Private Members

		/// <summary>
		/// Determines if all required handlers are attached.
		/// </summary>
		private bool _isAttached;

		/// <summary>
		/// Determines if the mouse activity is local to the <see cref="TriggerBase{T}.AssociatedObject"/>.
		/// </summary>
		private bool _isLocal;

		#endregion


		#region Public Properties

		public bool CanTrigger
		{
			get { return (bool) GetValue(CanTriggerProperty); }
			set { SetValue(CanTriggerProperty, value); }
		}

		public static readonly DependencyProperty CanTriggerProperty = DependencyProperty.Register(
			"CanTrigger",
			typeof (bool),
			typeof (RootTouchTrigger),
			null);

		#endregion


		#region Private Methods

		/// <summary>
		/// Ensures all required handlers are attached.
		/// </summary>
		private void EnsureAttached()
		{
			// Don't continue if everything is already attached
			if (_isAttached)
			{
				return;
			}


			var app = Application.Current;

			// Don't do anything if application is dead
			if (app == null)
			{
				return;
			}

			var rootVisual = app.RootVisual;

			// Don't do anything if RootVisual is dead
			if (rootVisual == null)
			{
				return;
			}

			_isAttached = true;


			// Attach to root visual
			rootVisual.AddHandler(
				UIElement.MouseLeftButtonDownEvent,
				new MouseButtonEventHandler(OnRootEvent),
				true
				);


			// Attach to associated UIElement
			AssociatedObject.AddHandler(
				UIElement.MouseLeftButtonDownEvent,
				new MouseButtonEventHandler(OnLocalEvent),
				true
				);
		}

		#endregion


		#region Event Handling

		/// <summary>
		/// Called after the trigger is attached to an AssociatedObject.
		/// </summary>
		protected override void OnAttached()
		{
			base.OnAttached();
			EnsureAttached();

			AssociatedObject.Loaded += (s, t) => EnsureAttached();
		}

		/// <summary>
		/// Called when the trigger is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		protected override void OnDetaching()
		{
			base.OnDetaching();

			var app = Application.Current;

			// Don't do anything if application is dead
			if (app == null)
			{
				return;
			}


			var rootVisual = app.RootVisual;

			// Don't do anything if RootVisual is dead
			if (rootVisual == null)
			{
				return;
			}


			// Attach to root visual
			rootVisual.RemoveHandler(
				UIElement.MouseLeftButtonDownEvent,
				new MouseButtonEventHandler(OnRootEvent)
				);


			// Attach to associated UIElement
			AssociatedObject.RemoveHandler(
				UIElement.MouseLeftButtonDownEvent,
				new MouseButtonEventHandler(OnLocalEvent)
				);

			_isAttached = false;
		}

		/// <summary>
		/// Called when the left mouse button is down on top of the <see cref="TriggerBase{T}.AssociatedObject"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLocalEvent(object sender, MouseButtonEventArgs e)
		{
			_isLocal = true;
		}

		/// <summary>
		/// Called when the left mouse button is down on top of the <see cref="Application.RootVisual"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRootEvent(object sender, MouseButtonEventArgs e)
		{
			// Don't continue if click was local to the associated object
			if (_isLocal)
			{
				_isLocal = false;
				return;
			}
			if (CanTrigger)
			{
				InvokeActions(null);
			}
		}

		#endregion
	}
}
