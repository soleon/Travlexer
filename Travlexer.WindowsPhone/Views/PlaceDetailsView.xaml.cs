using System;
using System.Windows.Controls;
using Codify.Attributes;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
	[ViewModelType(typeof(PlaceDetailsViewModel))]
	public partial class PlaceDetailsView
	{
		public PlaceDetailsView()
		{
			InitializeComponent();
		}

		private void SelectAllOnTextBoxGotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null)
			{
				throw new ArgumentException("Sender must be a TextBox.", "sender");
			}
			textBox.SelectAll();
		}
	}
}
