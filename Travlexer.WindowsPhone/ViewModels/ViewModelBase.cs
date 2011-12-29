using Travlexer.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.ViewModels
{
	public abstract class ViewModelBase : ModelBase, IViewModel
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the logical parent view model that owns this view model.
		/// </summary>
		public IViewModel Parent { get; set; }

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
