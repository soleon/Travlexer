using System;
using System.Windows;
using Codify.Extensions;
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

        public static void ShowMarketplaceReview()
        {
            new MarketplaceReviewTask().Show();
        }

        public static void SaveContact(
            string company,
            string firstName,
            string homeAddressCity,
            string homeaddressCountry,
            string homeAddressState,
            string homeAddressZipCode,
            string homePhone,
            string jobTitle,
            string lastName,
            string middleName,
            string mobilePhone,
            string nickname,
            string notes,
            string otherEmail,
            string personalEmail,
            string suffix,
            string title,
            string webSite,
            string workAddressCity,
            string workAddressCountry,
            string workAddressState,
            string workAddressStreet,
            string workAddressZipCode,
            string workEmail,
            string workPhone)
        {
            new SaveContactTask
            {
                Company = company,
                FirstName = firstName,
                HomeAddressCity = homeAddressCity,
                HomeAddressCountry = homeaddressCountry,
                HomeAddressState = homeAddressState,
                HomeAddressZipCode = homeAddressZipCode,
                HomePhone = homePhone,
                JobTitle = jobTitle,
                LastName = lastName,
                MiddleName = middleName,
                MobilePhone = mobilePhone,
                Nickname = nickname,
                Notes = notes,
                OtherEmail = otherEmail,
                PersonalEmail = personalEmail,
                Suffix = suffix,
                Title = title,
                Website = webSite,
                WorkAddressCity = workAddressCity,
                WorkAddressCountry = workAddressCountry,
                WorkAddressState = workAddressState,
                WorkAddressStreet = workAddressStreet,
                WorkAddressZipCode = workAddressZipCode,
                WorkEmail = workEmail,
                WorkPhone = workPhone
            }.Show();
        }

        public static void SelectContactAddress(Action<string, Exception> addressSelected)
        {
            var task = new AddressChooserTask();
            task.Completed += (s, e) => addressSelected.ExecuteIfNotNull(e.Address, e.Error);
            task.Show();
        }
    }
}