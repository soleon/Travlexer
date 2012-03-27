using System;
using Codify.Models;

namespace Codify.ViewModels
{
	public class DataViewModel<TData, TParent> : ViewModelBase<TParent>
		where TData : class, IModel
		where TParent : class, IViewModel
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataViewModel{TData, TParent}"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="parent">The logical parent view model that owns this view model.</param>
		public DataViewModel(TData data, TParent parent = null)
			: base(parent)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "The data model object is required to create a data view model.");
			}
			Data = data;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the <see cref="IModel"/> object that contains essential data of this <see cref="IViewModel{TParent}"/>.
		/// </summary>
		public TData Data { get; private set; }

		#endregion


		#region Event Handling

		protected override void OnDispose()
		{
			var data = Data;
			Data = default(TData);
			data.Dispose();
			base.OnDispose();
		}

		#endregion
	}

	public class DataViewModel<T> : ViewModelBase where T : IModel
	{
		#region Public Properties

		/// <summary>
		/// Gets the <see cref="IModel"/> object that contains essential data of this <see cref="IViewModel{TParent}"/>.
		/// </summary>
		public T Data { get; set; }

		#endregion
	}
}
