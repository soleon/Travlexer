using System;
using System.ComponentModel;
using Codify.WindowsPhone.Extensions;

namespace Codify.WindowsPhone.Models
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

	public class ObservableValue<T>
	{
		#region Constructors

		public ObservableValue(T value)
		{
			_value = value;
		}

		#endregion


		#region Public Events

		public event Action<T, T> ValueChanged;

		/// <summary>
		/// Occurs before <see cref="Value"/> is changed.
		/// </summary>
		/// <remarks>This event accepts a function as a handler, which takes the old value and the new value of this observable value as parameters and returns a the final value to be set.</remarks>
		public event Func<T, T, T> ValueChanging;

		#endregion


		#region Private Methods

		private void RaiseValueChanged(T oldValue, T newValue)
		{
			ValueChanged.ExecuteIfNotNull(oldValue, newValue);
		}

		#endregion


		#region Public Properties

		public T Value
		{
			get { return _value; }
			set
			{
				if (Equals(_value, value))
				{
					return;
				}
				var old = _value;
				var @new = ValueChanging.ExecuteIfNotNull(old, value, value);
				if (Equals(old, @new))
				{
					return;
				}
				_value = @new;
				RaiseValueChanged(old, @new);
			}
		}

		private T _value;

		#endregion
	}
}
