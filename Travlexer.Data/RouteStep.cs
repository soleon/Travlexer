namespace Travlexer.Data
{
    public class RouteStep
    {
        public int Distance { get; set; }

        public Location StartLocation { get; set; }

        public Location EndLocation { get; set; }

        public string Instruction { get; set; }
    }
}