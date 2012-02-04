using Codify.Models;

namespace Codify.ViewModels
{
	public interface IViewModel : IModel
	{
		/// <summary>
		/// Gets or sets the logical parent view model that owns this view model.
		/// </summary>
		IViewModel Parent { get; set; }
	}
}