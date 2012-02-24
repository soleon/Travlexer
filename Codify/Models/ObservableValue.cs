using System;
using Codify.Extensions;

namespace Codify.Models
{
	public class ObservableValue<T> : NotifyBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableValue{T}"/> class.
		/// </summary>
		public ObservableValue() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableValue{T}"/> class.
		/// </summary>
		/// <param name="defaultValue">The default value for <see cref="Value"/> property.</param>
		/// <remarks>Supplying the default value does not raise the <see cref="ValueChanged"/> event.</remarks>
		public ObservableValue(T defaultValue)
		{
			_value = defaultValue;
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
				RaisePropertyChanged(ValueProperty);
				RaiseValueChanged(old, @new);
			}
		}

		private T _value;
		private const string ValueProperty = "Value";

		#endregion
	}
}
