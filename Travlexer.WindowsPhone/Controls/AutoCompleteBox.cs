using System.Windows.Controls;

namespace Travlexer.WindowsPhone.Controls
{
    public class AutoCompleteBox : Microsoft.Phone.Controls.AutoCompleteBox
    {
        private const string PART_TextBox = "Text";

        public override void OnApplyTemplate()
        {
            var textBox = GetTemplateChild(PART_TextBox) as TextBox;
            if (textBox != null)
            {
                textBox.GotFocus += (s, e) => textBox.SelectAll();
                GotFocus += (s, e) => textBox.Focus();
            }
            base.OnApplyTemplate();
        }
    }
}