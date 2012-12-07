using System;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace Codify.WindowsPhone
{
    public static class PhoneTasks
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

        public static void SendEmail(string to, string subject, string body = null, string cc = null, string bcc = null, int? codePage = null)
        {
            new EmailComposeTask
            {
                To = to,
                Subject = subject,
                Body = body,
                Cc = cc,
                Bcc = bcc,
                CodePage = codePage
            }.Show();
        }
    }
}