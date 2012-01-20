namespace Codify.WindowsPhone.Services
{
	/// <summary>
	/// An enumeration of all possible statuses for a service callback.
	/// </summary>
	public enum CallbackStatus : byte
	{
		/// <summary>
		/// An unknown error occurred either on the service or client.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// The call completed successfully.
		/// </summary>
		Successful,

		/// <summary>
		/// The call was cancelled by the client.
		/// </summary>
		Cancelled,

		/// <summary>
		/// The service call raised an exception.
		/// </summary>
		ServiceException,

		/// <summary>
		/// There's no network connection available.
		/// </summary>
		NetworkUnavailable,

		/// <summary>
		/// The response was valid but the response data contains no result.
		/// </summary>
		EmptyResult,

		/// <summary>
		/// The request of this callback was invalid.
		/// </summary>
		InvalidRequest
	}
}