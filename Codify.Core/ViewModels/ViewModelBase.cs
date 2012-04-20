using Codify.Entities;

namespace Codify.ViewModels
{
    /// <summary>
    /// Represents a view model that requires a parent view model.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent.</typeparam>
    public abstract class ViewModelBase<TParent> : ModelBase, IViewModel<TParent> where TParent : class, IViewModel
    {
        private const string ParentProperty = "Parent";
        private TParent _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase{TParent}"/> class.
        /// </summary>
        /// <param name="parent">The logical parent view model that owns this view model.</param>
        protected ViewModelBase(TParent parent = null)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets or sets the logical parent view model that owns this view model.
        /// </summary>
        public TParent Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value, ParentProperty); }
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
            Parent = null;
        }
    }

    /// <summary>
    /// Represents a view model that does not require a parent view model.
    /// </summary>
    public abstract class ViewModelBase : ModelBase, IViewModel
    {
        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose() {}
    }
}