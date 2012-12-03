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
        }

        public uint Index { get; private set; }

        public string Distance
        {
            get { return Data.Distance.ToDistanceText(); }
        }

        public string Duration
        {
            get { return Data.Duration.ToDurationText(); }
        }
    }
}