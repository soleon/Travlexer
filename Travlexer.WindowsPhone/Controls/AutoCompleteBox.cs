using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Travlexer.WindowsPhone.Controls
{
	public class AutoCompleteBox : Microsoft.Phone.Controls.AutoCompleteBox
	{
		private const string PART_TextBox = "Text";
		private const string PART_Popup = "Popup";

		public override void OnApplyTemplate()
		{
			var textBox = GetTemplateChild(PART_TextBox) as TextBox;
			if (textBox != null)
			{
				textBox.GotFocus += (s, e) => textBox.SelectAll();
				GotFocus += (s, e) => textBox.Focus();
			}

			var popup = GetTemplateChild(PART_Popup) as Popup;
			if (popup != null)
			{
				popup.RenderTransform = new TranslateTransform { Y = PopupVerticalOffset};
			}
			base.OnApplyTemplate();
		}

		public double PopupVerticalOffset { get; set; }
	}
}
