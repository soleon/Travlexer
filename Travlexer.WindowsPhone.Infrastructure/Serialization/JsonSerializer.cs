using Codify.Serialization;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Infrastructure.Serialization
{
	public class JsonSerializer : ISerializer<string>
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
		public TTarget Deserialize<TTarget>(string source) where TTarget : class
		{
			return JsonConvert.DeserializeObject<TTarget>(source);
		}

		/// <summary>
		/// Tries to deserialize the source object to <see cref="TTarget"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="source">The source to deserialize.</param>
		/// <param name="output">The deserialized object as <see cref="TTarget"/> if deserialization is successful.</param>
		/// <returns>
		///   <c>true</c> if the deserialization process is successful, otherwise, <c>false</c>.
		/// </returns>
		public bool TryDeserialize<TTarget>(string source, out TTarget output) where TTarget : class
		{
			try
			{
				output = Deserialize<TTarget>(source);
				return true;
			}
			catch
			{
				return Fail(out output);
			}
		}

		/// <summary>
		/// Serializes the specified target into <see cref="string"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="target">The target to serialize.</param>
		/// <returns>
		/// The serialized representation of the target as <see cref="string"/>.
		/// </returns>
		public string Serialize<TTarget>(TTarget target) where TTarget : class
		{
			return JsonConvert.SerializeObject(target);
		}

		/// <summary>
		/// Tries to serialize the target into <see cref="string"/>.
		/// </summary>
		/// <typeparam name="TTarget">The type of the target.</typeparam>
		/// <param name="target">The target to be serialized.</param>
		/// <param name="output">The serialized representation of the source as <see cref="string"/> if serialization is successful.</param>
		/// <returns>
		///   <c>true</c> if the serialization process is successful, otherwise, <c>false</c>.
		/// </returns>
		public bool TrySerialize<TTarget>(TTarget target, out string output) where TTarget : class
		{
			try
			{
				output = Serialize(target);
				return true;
			}
			catch
			{
				return Fail(out output);
			}
		}

		#endregion


		#region Private Methods

		private static bool Fail<T>(out T output)
		{
			output = default(T);
			return false;
		}

		#endregion
	}
}