using Codify;
using Codify.Entities;

namespace Travlexer.WindowsPhone.Infrastructure
{
    public interface IConfigurationContext
    {
        /// <summary>
        ///     Gets the observable value that indicates whether this data context is doing any loading.
        /// </summary>
        ObservableValue<bool> IsBusy { get; }

        /// <summary>
        ///     Gets the state of the toolbar.
        /// </summary>
        ObservableValue<ExpansionStates> ToolbarState { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is tracking current location.
        /// </summary>
        ObservableValue<bool> IsTrackingCurrentLocation { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is offline.
        /// </summary>
        ObservableValue<bool> IsOnline { get; }

        /// <summary>
        ///     Toggles the state of the toolbar.
        /// </summary>
        void ToggleToolbarState();

        /// <summary>
        ///     Saves the data context to the storage provided by <see cref="ConfigurationContext.StorageProvider" />.
        /// </summary>
        void SaveContext();

        /// <summary>
        ///     Loads the data context from the storage provided by <see cref="ConfigurationContext.StorageProvider" />.
        /// </summary>
        void LoadContext();
    }
}