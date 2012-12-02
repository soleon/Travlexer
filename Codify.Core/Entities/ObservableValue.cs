using System;
using Codify.Extensions;

namespace Codify.Entities
{
    public class ObservableValue<T> : NotifyableEntity
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableValue{T}" /> class.
        /// </summary>
        public ObservableValue() {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableValue{T}" /> class.
        /// </summary>
        /// <param name="defaultValue">
        ///     The default value for <see cref="Value" /> property.
        /// </param>
        /// <param name="ignoreValueEqualityCheck">
        ///     if set to <c>true</c> ignores value equality check and always notify value change.
        /// </param>
        /// <remarks>
        ///     Supplying the default value does not raise the <see cref="ValueChanged" /> event.
        /// </remarks>
        public ObservableValue(T defaultValue, bool ignoreValueEqualityCheck = false)
        {
            _value = defaultValue;
            IgnoreValueEqualityCheck = ignoreValueEqualityCheck;
        }

        #endregion


        #region Public Events

        public event Action<T, T> ValueChanged;

        /// <summary>
        ///     Occurs before <see cref="Value" /> is changed.
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
                if (!IgnoreValueEqualityCheck && Equals(_value, value))
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

        /// <summary>
        ///     Gets or sets a value indicating whether this instance raises value change related events even if the value is set to the same.
        /// </summary>
        public bool IgnoreValueEqualityCheck { get; set; }

        #endregion
    }
}