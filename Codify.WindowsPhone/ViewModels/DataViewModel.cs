using System;
using Codify.WindowsPhone.Models;

namespace Codify.WindowsPhone.ViewModels
{
	public class DataViewModel<T> : ViewModelBase where T : IModel
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataViewModel{T}"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="parent">The logical parent view model that owns this view model.</param>
		public DataViewModel(T data, IViewModel parent = null)
			: base(parent)
		{
			if (data.Equals(null))
			{
				throw new ArgumentNullException("data", "The data model object is required to create a data view model.");
			}
			Data = data;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the <see cref="IModel"/> object that contains essential data of this <see cref="IViewModel"/>.
		/// </summary>
		public T Data { get; private set; }

		#endregion


		#region Event Handling

		protected override void OnDispose()
		{
			var data = Data;
			Data = default(T);
			data.Dispose();
			base.OnDispose();
		}

		#endregion
	}
}