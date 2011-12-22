using System;
using System.ComponentModel;
using Travlexer.Core.Extensions;

namespace Travlexer.Core
{
	public abstract class EntityBase : INotifyPropertyChanged, IDisposable
	{
		#region Public Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


		#region Event Handling

		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;
			handler.ExecuteIfNotNull(this, e);
		}

		public void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		public void OnPropertyChanged(params string[] propertyNames)
		{
			foreach (var propertyName in propertyNames)
			{
				OnPropertyChanged(propertyName);
			}
		}

		protected virtual void OnDispose() { }

		#endregion


		#region Public Methods

		public void Dispose()
		{
			OnDispose();
		}

		#endregion


		#region Protected Methods

		protected bool SetProperty<T>(ref T property, T value, string propertyName)
		{
			if (property.Equals(value))
			{
				return false;
			}
			property = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected bool SetProperty<T>(ref T property, T value, params string[] propertyNames)
		{
			if (property.Equals(value))
			{
				return false;
			}
			property = value;
			OnPropertyChanged(propertyNames);
			return true;
		}

		#endregion
	}
}
