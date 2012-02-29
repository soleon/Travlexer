using Codify.Models;

namespace Codify.ViewModels
{
	/// <summary>
	/// Represents a view model that requires a parent view model.
	/// </summary>
	/// <typeparam name="TParent">The type of the parent view model.</typeparam>
	public interface IViewModel<TParent> : IViewModel
	{
		/// <summary>
		/// Gets or sets the logical parent view model that owns this view model.
		/// </summary>
		TParent Parent { get; set; }
	}

	/// <summary>
	/// Represents a view model without the need of a parent view model.
	/// </summary>
	public interface IViewModel : IModel
	{

	}
}