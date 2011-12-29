namespace Travlexer.WindowsPhone
{
	public static class Globals
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the data context that contains all essential data for this application.
		/// </summary>
		public static IDataContext DataContext
		{
			get { return _dataContext ?? (_dataContext = new DataContext()); }
			set { _dataContext = value; }
		}

		private static IDataContext _dataContext;

		#endregion
	}
}
