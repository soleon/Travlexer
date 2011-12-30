using Travelexer.WindowsPhone.Core.Models;

namespace Travelexer.WindowsPhone.Core.ViewModels
{
	public interface IViewModel : IModel
	{
		/// <summary>
		/// Gets or sets the logical parent view model that owns this view model.
		/// </summary>
		IViewModel Parent { get; set; }
	}
}