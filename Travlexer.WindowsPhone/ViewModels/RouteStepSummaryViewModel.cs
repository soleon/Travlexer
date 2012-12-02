using Codify.ViewModels;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class RouteStepSummaryViewModel : DataViewModel<RouteStep>
    {
        public RouteStepSummaryViewModel(uint index, RouteStep step)
        {
            Index = index;
            Data = step;
            Distance = step.DistanceText;
            Duration = step.DurationText;
        }

        public uint Index { get; private set; }

        public string Distance { get; private set; }

        public string Duration { get; private set; }
    }
}