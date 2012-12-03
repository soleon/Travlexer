namespace Travlexer.Data
{
    public class RouteStep
    {
        /// <summary>
        ///     Gets or sets the distance in meters.
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        ///     Gets or sets the duration in seconds.
        /// </summary>
        public int Duration { get; set; }

        public Location StartLocation { get; set; }

        public Location EndLocation { get; set; }

        public string Instruction { get; set; }
    }
}