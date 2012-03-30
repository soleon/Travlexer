using System.Windows;

namespace Codify.WindowsPhone
{
	public class PhoneApplicationPage : Microsoft.Phone.Controls.PhoneApplicationPage
	{
		#region DataContext

		public new object DataContext
		{
			get { return GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}

		/// <summary>
		/// Defines the <see cref="DataContext"/> dependency property.
		/// </summary>
		public new static readonly DependencyProperty DataContextProperty = DependencyProperty.Register(
			"DataContext",
			typeof (object),
			typeof(PhoneApplicationPage),
			new PropertyMetadata(default(object), (s, e) => ((PhoneApplicationPage)s).OnDataContextChanged(e.OldValue, e.NewValue)));

		/// <summary>
		/// Called when <see cref="DataContext"/> changes.
		/// </summary>
		protected virtual void OnDataContextChanged(object oldValue, object newValue)
		{
			base.DataContext = newValue;
		}

		#endregion
	}
}
