using System;

namespace Codify.ViewModels
{
    public class DataViewModel<TData, TParent> : ViewModelBase<TParent> where TParent : class where TData : class
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataViewModel{TData, TParent}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="parent">The logical parent view model that owns this view model.</param>
        public DataViewModel(TData data, TParent parent = null)
            : base(parent)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", "The data object is required to create a data view model.");
            }
            Data = data;
        }

        #endregion


        #region Public Properties

        /// <summary>
        ///     Gets the object that contains essential data of this <see cref="IViewModel{TParent}" />.
        /// </summary>
        public TData Data { get; private set; }

        #endregion


        #region Event Handling

        protected override void OnDispose()
        {
            Data = default(TData);
            base.OnDispose();
        }

        #endregion
    }
}