using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Travlexer.WindowsPhone
{
	public partial class App
	{
		#region Public Properties

		/// <summary>
		/// Provides easy access to the root frame of the Phone Application.
		/// </summary>
		/// <returns>The root frame of the Phone Application.</returns>
		public PhoneApplicationFrame RootFrame { get; private set; }

		#endregion


		#region Constructors

		/// <summary>
		/// Constructor for the Application object.
		/// </summary>
		public App()
		{
			// Global handler for uncaught exceptions. 
			UnhandledException += OnApplicationUnhandledException;

			// Standard Silverlight initialization
			InitializeComponent();

			// Phone-specific initialization
			InitializePhoneApplication();

			// Show graphics profiling information while debugging.
			if (Debugger.IsAttached)
			{
				// Display the current frame rate counters.
				// Current.Host.Settings.EnableFrameRateCounter = true;

				// Show the areas of the app that are being redrawn in each frame.
				// Current.Host.Settings.EnableRedrawRegions = true;

				// Enable non-production analysis visualization mode, 
				// which shows areas of a page that are handed off to GPU with a colored overlay.
				// Current.Host.Settings.EnableCacheVisualization = true;

				// Disable the application idle detection by setting the UserIdleDetectionMode property of the
				// application's PhoneApplicationService object to Disabled.
				// Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
				// and consume battery power when the user is not using the phone.
				PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
			}
		}

		#endregion


		#region Event Handling

		private void OnApplicationLaunching(object sender, LaunchingEventArgs e)
		{
			ApplicationContext.LoadContext();
		}

		private void OnApplicationActivated(object sender, ActivatedEventArgs e)
		{
			if (!e.IsApplicationInstancePreserved)
			{
				ApplicationContext.LoadContext();
			}
		}

		private void OnApplicationDeactivated(object sender, DeactivatedEventArgs e)
		{
			ApplicationContext.SaveContext();
		}

		private void OnApplicationClosing(object sender, ClosingEventArgs e)
		{
			ApplicationContext.SaveContext();
		}

		private static void OnRootFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if (Debugger.IsAttached)
			{
				// A navigation has failed; break into the debugger
				Debugger.Break();
			}
		}

		private void OnApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				Debugger.Break();
			}
			else
			{
#if DEBUG
				var n = Environment.NewLine;
				var debug =
					e.ExceptionObject.Message + n + n +
					RootFrame.CurrentSource + n + e.ExceptionObject.StackTrace;
				MessageBox.Show(debug);
#endif
			}
		}

		#endregion


		#region Phone application initialization

		// Avoid double-initialization
		private bool _phoneApplicationInitialized;

		private void InitializePhoneApplication()
		{
			if (_phoneApplicationInitialized)
			{
				return;
			}


			// Create the frame but don't set it as RootVisual yet; this allows the splash
			// screen to remain active until the application is ready to render.
			RootFrame = new TransitionFrame();
			RootFrame.Navigated += OnCompleteInitializePhoneApplication;

			// Handle navigation failures
			RootFrame.NavigationFailed += OnRootFrameNavigationFailed;

			// Ensure we don't initialize again
			_phoneApplicationInitialized = true;

			// Set the root visual to allow the application to render
			RootVisual = RootFrame;

			// Initialize application context.
			ApplicationContext.Initialize(RootFrame);
		}

		//// Do not add any additional code to this method
		private void OnCompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
		{
			// Remove this handler since it is no longer needed
			RootFrame.Navigated -= OnCompleteInitializePhoneApplication;
		}

		#endregion
	}
}
