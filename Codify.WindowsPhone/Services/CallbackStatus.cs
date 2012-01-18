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
		/// Connection to the service was not found.
		/// </summary>
		ConnectionFailed,

		/// <summary>
		/// The service call raised an exception.
		/// </summary>
		ServiceException,

		/// <summary>
		/// The call was denied by the server.
		/// </summary>
		AccessDenied,

		/// <summary>
		/// The current session is invalid or expired.
		/// </summary>
		InvalidOrExpiredSession,

		/// <summary>
		/// There's no network connection available.
		/// </summary>
		NetworkUnavailable
	}
}