using Codify.ViewModels;

namespace Travlexer.WindowsPhone.ViewModels
{
    /// <summary>
    /// A <see cref="DataViewModel{T}"/> that has a build-in <see cref="IsChecked"/> field.
    /// </summary>
    public class CheckableViewModel<T> : DataViewModel<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetValue(ref _isChecked, value, IsCheckedProperty); }
        }

        private bool _isChecked;
        private const string IsCheckedProperty = "IsChecked";
    }
}