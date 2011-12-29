using System;

namespace Travlexer.WindowsPhone.Services
{
	/// <summary>
	/// Arguments passed to a service callback.
	/// </summary>
	public class CallbackEventArgs : EventArgs
	{
		#region Public Properties

		/// <summary>
		/// Gets the status of the service call.
		/// </summary>
		public CallbackStatus Status { get; private set; }

		/// <summary>
		/// Gets the exception raised during the service call.
		/// </summary>
		public Exception Exception { get; private set; }

		#endregion


		#region Constructors

		/// <summary>
		/// Creates a new instance of <see cref="CallbackEventArgs"/>.
		/// </summary>
		/// <param name="status">The status of the service call.</param>
		/// <param name="exception">The associated exception, if any.</param>
		public CallbackEventArgs(CallbackStatus status = CallbackStatus.Successful, Exception exception = null)
		{
			Status = status;
			Exception = exception;
		}

		#endregion
	}

	/// <summary>
	/// Arguments passed to a service callback.
	/// </summary>
	/// <typeparam name="TResult">The type of the results expected from the service call.</typeparam>
	public class CallbackEventArgs<TResult> : CallbackEventArgs
	{
		#region Public Properties

		/// <summary>
		/// Gets the data retrieved from the service.
		/// </summary>
		public TResult Result { get; private set; }

		#endregion


		#region Constructors

		/// <summary>
		/// Creates a new instance of <see cref="CallbackEventArgs{T}"/>.
		/// </summary>
		/// <param name="result">The data retrieved from the service.</param>
		public CallbackEventArgs(TResult result)
		{
			Result = result;
		}

		/// <summary>
		/// Creates a new instance of <see cref="CallbackEventArgs{T}"/>.
		/// </summary>
		/// <param name="status">The status of the service call.</param>
		/// <param name="exception">The exception raised, if any.</param>
		public CallbackEventArgs(CallbackStatus status, Exception exception = null)
			: base(status, exception) {}

		#endregion
	}

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
