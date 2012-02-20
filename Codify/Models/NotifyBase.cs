using System.ComponentModel;
using Codify.Extensions;

namespace Codify.Models
{
	public abstract class NotifyBase : INotifyPropertyChanged
	{
		#region Public Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


		#region Protected Methods

		protected void RaisePropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			handler.ExecuteIfNotNull(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
