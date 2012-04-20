using Serialization;

namespace Codify.Serialization
{
	public class BinarySerializer : SerializerBase<byte[]>
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
		public override TTarget Deserialize<TTarget>(byte[] source)
		{
			return SilverlightSerializer.Deserialize<TTarget>(source);
		}

		/// <summary>
		/// Serializes the specified target into <see cref="byte[]"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="target">The target to serialize.</param>
		/// <returns>
		/// The serialized representation of the target as <see cref="byte[]"/>.
		/// </returns>
		public override byte[] Serialize<TTarget>(TTarget target)
		{
			return SilverlightSerializer.Serialize(target);
		}

		#endregion
	}
}
