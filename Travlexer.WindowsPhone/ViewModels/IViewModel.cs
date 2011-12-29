using Travlexer.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.ViewModels
{
	public interface IViewModel : IModel
	{
		/// <summary>
		/// Gets or sets the logical parent view model that owns this view model.
		/// </summary>
		IViewModel Parent { get; set; }
	}
}