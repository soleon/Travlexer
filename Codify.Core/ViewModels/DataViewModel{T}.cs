using Codify.Entities;

namespace Codify.ViewModels
{
    public class DataViewModel<T> : NotifyableEntity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the object that contains essential data of this view model.
        /// </summary>
        public T Data { get; set; }

        #endregion
    }
}