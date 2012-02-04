using System;
using Codify.Extensions;

namespace Codify.Models
{
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