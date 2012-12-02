namespace Travlexer.Data
{
    public class RouteStep
    {
        public int Distance { get; set; }

        public string DistanceText { get; set; }

        /// <summary>
        ///     Gets or sets the duration in seconds.
        /// </summary>
        public int Duration { get; set; }

        public string DurationText { get; set; }

        public Location StartLocation { get; set; }

        public Location EndLocation { get; set; }

        public string Instruction { get; set; }
    }
}