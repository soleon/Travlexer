using Codify.Commands;
using Codify.Entities;
using Codify.WindowsPhone;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class SettingsViewModel : NotifyableEntity
    {
        private static readonly UnitSystems[] StaticUnitSystems = new[] {Infrastructure.UnitSystems.Metric, Infrastructure.UnitSystems.Imperial};

        private readonly IDataContext _data;

        public SettingsViewModel()
        {
            _data = ApplicationContext.Data;
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

        public ObservableValue<bool> HideToolbar
        {
            get { return _data.HideToolbar; }
        }
    }
}