using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Codify.Storage
{
	public class IsolatedStorage : IStorage
	{
		#region Public Methods

		public void SaveSetting<T>(string key, T value)
		{
			IsolatedStorageSettings.ApplicationSettings[key] = value;
		}

		public bool TryGetSetting<T>(string key, out T value)
		{
			try
			{
				return IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);
			}
			catch
			{
				value = default(T);
				return false;
			}
		}

		#endregion


		#region Public Properties

		public IDictionary<string, object> Settings
		{
			get { return IsolatedStorageSettings.ApplicationSettings; }
		}

		#endregion
	}
}
