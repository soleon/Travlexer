using Newtonsoft.Json;

namespace Codify.Serialization
{
    public class JsonSerializer : SerializerBase<string>
    {
        #region Public Methods

        /// <summary>
        /// Deserializes the specified source to <see cref="TTarget"/>.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="source">The source to deserialize.</param>
        /// <returns>
        /// The deserialized object as <see cref="TTarget"/>.
        /// </returns>
        public override TTarget Deserialize<TTarget>(string source)
        {
            return JsonConvert.DeserializeObject<TTarget>(source);
        }

        /// <summary>
        /// Serializes the specified target into <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="target">The target to serialize.</param>
        /// <returns>
        /// The serialized representation of the target as <see cref="string"/>.
        /// </returns>
        public override string Serialize<TTarget>(TTarget target)
        {
            return JsonConvert.SerializeObject(target);
        }


        #endregion
    }
}