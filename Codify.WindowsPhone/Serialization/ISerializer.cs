namespace Codify.WindowsPhone.Serialization
{
	public interface ISerializer<TSource>
	{
		#region Methods

		/// <summary>
		/// Deserializes the specified source to <see cref="TTarget"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="source">The source to deserialize.</param>
		/// <returns>
		/// The deserialized object as <see cref="TTarget"/>.
		/// </returns>
		TTarget Deserialize<TTarget>(TSource source) where TTarget : class;

		/// <summary>
		/// Tries to deserialize the source object to <see cref="TTarget"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="source">The source to deserialize.</param>
		/// <param name="output">The deserialized object as <see cref="TTarget"/> if deserialization is successful.</param>
		/// <returns>
		///   <c>true</c> if the deserialization process is successful, otherwise, <c>false</c>.
		/// </returns>
		bool TryDeserialize<TTarget>(TSource source, out TTarget output) where TTarget : class;

		/// <summary>
		/// Serializes the specified target into <see cref="TSource"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="target">The target to serialize.</param>
		/// <returns>
		/// The serialized representation of the target as <see cref="TSource"/>.
		/// </returns>
		TSource Serialize<TTarget>(TTarget target) where TTarget : class;

		/// <summary>
		/// Tries to serialize the target into <see cref="TSource"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="target">The target to be serialized.</param>
		/// <param name="output">The serialized representation of the source as <see cref="TSource"/> if serialization is successful.</param>
		/// <returns>
		///   <c>true</c> if the serialization process is successful, otherwise, <c>false</c>.
		/// </returns>
		bool TrySerialize<TTarget>(TTarget target, out TSource output) where TTarget : class;

		#endregion
	}
}