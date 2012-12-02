using System;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace Codify.WindowsPhone
{
    public class PhoneApplicationPage : Microsoft.Phone.Controls.PhoneApplicationPage
    {
        #region DataContext

        public new object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        /// <summary>
        ///     Defines the <see cref="DataContext" /> dependency property.
        /// </summary>
        public new static readonly DependencyProperty DataContextProperty = DependencyProperty.Register(
            "DataContext",
            typeof (object),
            typeof (PhoneApplicationPage),
            new PropertyMetadata(default(object), (s, e) => ((PhoneApplicationPage) s).OnDataContextChanged(e.OldValue, e.NewValue)));

        /// <summary>
        ///     Called when <see cref="DataContext" /> changes.
        /// </summary>
        protected virtual void OnDataContextChanged(object oldValue, object newValue)
        {
            base.DataContext = newValue;
        }

        #endregion
    }

    public static class Utilities
    {
        /// <summary>
        ///     Opens the URL in a <see cref="T:Microsoft.Phone.Tasks.WebBrowserTask" />.
        /// </summary>
        /// <param name="url">The URL to open in the browser task.</param>
        public static void OpenUrl(string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                new WebBrowserTask {Uri = uri}.Show();
            }
            else
            {
                MessageBox.Show("The web site address doesn't seem valid, we could not navigate to it.", "Incorrect Web Site", MessageBoxButton.OK);
            }
        }

        public static void CallPhoneNumber(string name, string number)
        {
            new PhoneCallTask {DisplayName = name, PhoneNumber = number}.Show();
        }
    }
}