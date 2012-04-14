using System.Collections.Generic;
namespace Codify.Storage
{
    public interface IStorage
    {
        #region Methods

        void SaveSetting<T>(string key, T value);

        bool TryGetSetting<T>(string key, out T value);

        #endregion


        #region Properties

        IDictionary<string, object> Settings { get; }

        #endregion
    }
}
