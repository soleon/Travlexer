using System.Collections.Generic;
using Codify.Entities;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class SettingsViewModel : NotifyableEntity
    {
        private static readonly UnitSystems[] StaticUnitSystems = new[]{Infrastructure.UnitSystems.Metric, Infrastructure.UnitSystems.Imperial, };

        private readonly IDataContext _data;

        public SettingsViewModel(IDataContext data)
        {
            _data = data;
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
    }
}