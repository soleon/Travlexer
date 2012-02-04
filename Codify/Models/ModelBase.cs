using System.ComponentModel;
using Codify.Extensions;

namespace Codify.Models
{
	public abstract class ModelBase : IModel
	{
		#region Public Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


		#region Public Methods

		public void Dispose()
		{
			OnDispose();
		}

		#endregion


		#region Event Handling

		protected virtual void OnDispose() {}

		#endregion


		#region Protected Methods

		protected void RaisePropertyChange(PropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;
			handler.ExecuteIfNotNull(this, e);
		}

		protected void RaisePropertyChange(string propertyName)
		{
			RaisePropertyChange(new PropertyChangedEventArgs(propertyName));
		}

		protected void RaisePropertyChange(params string[] propertyNames)
		{
			foreach (var propertyName in propertyNames)
			{
				RaisePropertyChange(propertyName);
			}
		}

		protected bool SetProperty<T>(ref T property, T value, string propertyName)
		{
			if (Equals(property, value))
			{
				return false;
			}
			property = value;
			RaisePropertyChange(propertyName);
			return true;
		}

		protected bool SetProperty<T>(ref T property, T value, params string[] propertyNames)
		{
			if (Equals(property, value))
			{
				return false;
			}
			property = value;
			RaisePropertyChange(propertyNames);
			return true;
		}

		#endregion
	}
}
