using System;
using Codify.Entities;

namespace Codify.ViewModels
{
    /// <summary>
    /// Represents a view model that requires a parent view model.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent.</typeparam>
    public abstract class ViewModelBase<TParent> : NotifyableEntity, IDisposable where TParent : class
    {
        private const string ParentProperty = "Parent";
        private TParent _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase{TParent}"/> class.
        /// </summary>
        /// <param name="parent">The logical parent view model that owns this view model.</param>
        protected ViewModelBase(TParent parent = default(TParent))
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets or sets the logical parent view model that owns this view model.
        /// </summary>
        public TParent Parent
        {
            get { return _parent; }
            set { SetValue(ref _parent, value, ParentProperty); }
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
            Parent = default(TParent);
        }
    }
}