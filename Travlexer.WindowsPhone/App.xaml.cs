using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Travlexer.WindowsPhone
{
    public partial class App
    {
        #region Public Properties

        /// <summary>
        ///     Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        #endregion


        #region Constructors

        /// <summary>
        ///     Constructor for the Application object.
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
            if (Debugger.IsAttached) Debugger.Break();
            try
            {
                var uiDispatcher = RootVisual.Dispatcher;
                var action = new Action(() =>
                {
                    var n2 = Environment.NewLine + Environment.NewLine;
                    if (MessageBox.Show("The application has encountered an error, we apologize for any inconvenience." + n2 + "Do you want to report this error via email?", "Oops...", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
                    var ex = e.ExceptionObject;
                    new EmailComposeTask
                    {
                        Body = ex + n2 + "OS: " + Environment.OSVersion + n2 + "Location: " + RootFrame.CurrentSource,
                        Subject = "Triplexer " + ApplicationContext.Data.AppVersion + " error report",
                        To = "codifying@gmail.com"
                    }.Show();
                });
                if (uiDispatcher.CheckAccess()) action();
                else uiDispatcher.BeginInvoke(action);
            }
            catch (Exception)
            {
                if (Debugger.IsAttached) throw;
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

            // App store policy requirement: ask user if location service is allowed at least once.
            if (ApplicationContext.Data.LastRanVersion == null)
                ApplicationContext.Data.UseLocationService.Value = MessageBox.Show("In order to display your location and provide optimal user experience, we need access to your current location. Your location information will not be stored or shared, and you can always disable this feature in the settings page. Do you want to enable access to location service?", "Location Service", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        #endregion
    }
}