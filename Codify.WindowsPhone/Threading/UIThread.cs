using System;
using System.Threading;
using System.Windows;
using Codify.WindowsPhone.Extensions;

namespace Codify.WindowsPhone.Threading
{
	/// <summary>
	/// A utility class used for synchronizing the UI thread
	/// </summary>
	public static class UIThread
	{
		#region Private Static Members

		/// <summary>
		/// Determines if the current deployment is in a test environment.
		/// </summary>
		private static bool? _isTestMode;

		#endregion


		#region Public Static Properties

		/// <summary>
		/// Determines if there is a UI application running
		/// </summary>
		/// <remarks>
		/// This is mostly used for testing, since test projects do not typically summon a UI Thread.
		/// </remarks>
		public static bool IsRunningOnUIThread
		{
			get
			{
				Deployment deployment = null;

				if (_isTestMode == null)
				{
					try
					{
						deployment = Deployment.Current;
						_isTestMode = false;
					}
					catch (TypeInitializationException)
					{
						_isTestMode = true;
					}
				}
				else if (!_isTestMode.GetValueOrDefault())
				{
					deployment = Deployment.Current;
				}

				return deployment == null || deployment.Dispatcher.CheckAccess();
			}
		}

		#endregion


		#region Public Static Methods

		/// <summary>
		/// Recoils the given action on the UI Thread under normal priority.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		public static void InvokeBack(Action action)
		{
			RunWorker(() => InvokeAsync(action));
		}

		/// <summary>
		/// Invokes the given action on the UI Thread under normal priority.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		public static void InvokeAsync(Action action)
		{
			if (action == null)
			{
				return;
			}

			if (!IsRunningOnUIThread)
			{
				Deployment.Current.Dispatcher.BeginInvoke(action);
			}
			else
			{
				action();
			}
		}

		/// <summary>
		/// Invokes the given action on a background thread
		/// </summary>
		/// <param name="work">An action executed without UI access</param>
		/// <param name="onCompleted">An action executed on the UI thread after work is completed</param>
		public static void RunWorker(Action work, Action onCompleted)
		{
			// Don't do anything if the work action is empty
			if (work == null)
			{
				return;
			}


			if (IsRunningOnUIThread)
			{
				// Spawn new thread only if UI is active
				ThreadPool.QueueUserWorkItem(
					workAction =>
					{
						((Action) workAction)();
						InvokeAsync(onCompleted);
					},
					work
					);
			}
			else
			{
				// Otherwise, just execute in linear fashion
				work();
				onCompleted.ExecuteIfNotNull();
			}
		}

		/// <summary>
		/// Invokes the given action on a background thread
		/// </summary>
		/// <param name="work">An action executed without UI access</param>
		public static void RunWorker(Action work)
		{
			RunWorker(work, null);
		}

		#endregion
	}
}
