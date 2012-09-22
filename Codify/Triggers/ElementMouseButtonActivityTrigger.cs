using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Codify.Triggers
{
    /// <summary>
    /// Executes a collection of actions when the <see cref="Application.RootVisual"/> raises any <see cref="FrameworkElement.MouseLeftButtonDown"/> activity.
    /// </summary>
    [Description("Executes a collection of action when the Application.RootVisual raises any MouseLeftButtonDown activity.")]
    public class ElementMouseButtonActivityTrigger : TriggerBase<FrameworkElement>
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

        #region CanTrigger
        public bool CanTrigger
        {
            get { return (bool)GetValue(CanTriggerProperty); }
            set { SetValue(CanTriggerProperty, value); }
        }

        public static readonly DependencyProperty CanTriggerProperty = DependencyProperty.Register(
            "CanTrigger",
            typeof(bool),
            typeof(ElementMouseButtonActivityTrigger),
            new PropertyMetadata(true));
        #endregion


        #region Element

        public FrameworkElement Element
        {
            get { return (FrameworkElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="Element"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element",
            typeof(FrameworkElement),
            typeof(ElementMouseButtonActivityTrigger),
            new PropertyMetadata(default(FrameworkElement), (s, e) => ((ElementMouseButtonActivityTrigger)s).OnElementChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue)));

        #endregion

        #endregion


        #region Private Methods

        /// <summary>
        /// Ensures all required handlers are attached.
        /// </summary>
        private void Attach(UIElement element)
        {
            // Don't continue if everything is already attached or the target element is null.
            if (_isAttached || element == null || AssociatedObject == null)
            {
                return;
            }

            _isAttached = true;


            // Attach to root visual
            element.AddHandler(
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

        private void Detach(UIElement element)
        {
            // Don't do anything if element is invalid.
            if (element == null || !_isAttached || AssociatedObject == null)
            {
                return;
            }


            // Attach to root visual
            element.RemoveHandler(
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

        #endregion


        #region Event Handling

        /// <summary>
        /// Called when <see cref="Element"/> changes.
        /// </summary>
        private void OnElementChanged(UIElement oldValue, UIElement newValue)
        {
            Detach(oldValue);
            Attach(newValue);
        }

        /// <summary>
        /// Called after the trigger is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            Attach(Element);
            AssociatedObject.Loaded += (s, e) => Attach(Element);
            base.OnAttached();
        }

        /// <summary>
        /// Called when the trigger is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            Detach(Element);
            base.OnDetaching();
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
