using System.Windows;
using Codify.Entities;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class SettingsViewModel : NotifyableEntity
    {
        private static readonly UnitSystems[] StaticUnitSystems = new[] {Infrastructure.UnitSystems.Metric, Infrastructure.UnitSystems.Imperial};
        private static readonly HorizontalAlignment[] StaticToolbarAlignments = new[] {HorizontalAlignment.Left, HorizontalAlignment.Right};

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

        public HorizontalAlignment[] ToolbarAlighments
        {
            get { return StaticToolbarAlignments; }
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

        public ObservableValue<HorizontalAlignment> ToolbarAlignment
        {
            get { return _data.ToolbarAlignment; }
        }
    }
}