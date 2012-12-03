using System;
using Codify.Commands;
using Codify.Entities;
using Microsoft.Phone.Tasks;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class SettingsViewModel : NotifyableEntity
    {
        private static readonly UnitSystems[] StaticUnitSystems = new[] {Infrastructure.UnitSystems.Metric, Infrastructure.UnitSystems.Imperial};
        
        private readonly EmailComposeTask _emailComposeTask;
        private readonly WebBrowserTask _webBrowserTask;
        private readonly IDataContext _data;

        public SettingsViewModel(IDataContext data)
        {
            _data = data;
            _emailComposeTask = new EmailComposeTask
            {
                To = "codifying@gmail.com",
                Subject = "About Travlexer"
            };
            _webBrowserTask = new WebBrowserTask
            {
                Uri = new Uri("http://codifying.wordpress.com", UriKind.Absolute)
            };
            CommandSendEmail = new DelegateCommand(_emailComposeTask.Show);
            CommandNavigateToWebSite = new DelegateCommand(_webBrowserTask.Show);
        }

        public ObservableValue<bool> UseMapAnimation
        {
            get { return _data.UseMapAnimation; }
        }

        public UnitSystems[] UnitSystems
        {
            get { return StaticUnitSystems; }
        }

        public ObservableValue<UnitSystems> SelectedUnitSystem
        {
            get { return _data.UnitSystem; }
        }

        public ObservableValue<bool> UseLocationService
        {
            get { return _data.UseLocationService; }
        }

        public ObservableValue<bool> PreventScreenLock
        {
            get { return _data.PreventScreenLock; }
        }

        public Version AppVersion
        {
            get { return _data.AppVersion; }
        }

        public DelegateCommand CommandSendEmail { get; private set; }

        public DelegateCommand CommandNavigateToWebSite { get; private set; }
    }
}