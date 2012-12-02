using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Codify.Behaviors
{
    public class RealTimeBindingBehavior : Behavior<TextBox>
    {
        #region Private Members

        private TextBox _textBox;
        private BindingExpression _expression;

        #endregion


        #region Event Handling

        protected override void OnAttached()
        {
            _textBox = AssociatedObject;
            if (_textBox != null)
            {
                _textBox.KeyUp += OnAssociatedTextBoxKeyUp;
                _expression = _textBox.GetBindingExpression(TextBox.TextProperty);
            }
            base.OnAttached();
        }

        private void OnAssociatedTextBoxKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (_expression != null)
            {
                _expression.UpdateSource();
            }
        }

        #endregion
    }
}