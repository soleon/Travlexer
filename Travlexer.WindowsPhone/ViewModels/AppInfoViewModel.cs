using System;
using Codify.Commands;
using Codify.Entities;
using Codify.WindowsPhone;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class AppInfoViewModel : NotifyableEntity
    {
        public AppInfoViewModel()
        {
            CommandSendEmail = new DelegateCommand(() => PhoneTasks.SendEmail("codifying@gmail.com", "Triplexer " + AppVersion + " feedback"));
        }

        public Version AppVersion
        {
            get { return ApplicationContext.Data.AppVersion; }
        }

        public DelegateCommand CommandSendEmail { get; private set; }
    }
}