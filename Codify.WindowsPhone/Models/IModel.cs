using System;
using System.ComponentModel;

namespace Codify.WindowsPhone.Models
{
	public interface IModel : INotifyPropertyChanged, IDisposable
	{
		#region Event Handling

		void OnPropertyChanged(PropertyChangedEventArgs e);

		void OnPropertyChanged(string propertyName);

		void OnPropertyChanged(params string[] propertyNames);

		#endregion
	}
}