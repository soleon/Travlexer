using Codify.Models;

namespace Codify.ViewModels
{
	public abstract class ViewModelBase : ModelBase, IViewModel
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelBase"/> class.
		/// </summary>
		/// <param name="parent">The logical parent view model that owns this view model.</param>
		protected ViewModelBase(IViewModel parent = null)
		{
			Parent = parent;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets or sets the logical parent view model that owns this view model.
		/// </summary>
		public IViewModel Parent
		{
			get { return _parent; }
			set { SetProperty(ref _parent, value, ParentProperty); }
		}
		private IViewModel _parent;
		private const string ParentProperty = "Parent";

		#endregion


		#region Protected Methods

		protected override void OnDispose()
		{
			Parent = null;
			base.OnDispose();
		}

		#endregion
	}
}
