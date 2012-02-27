namespace Codify.GoogleMaps.Entities
{
	public struct Size
	{
		public int Width { get; set; }
		public int Height { get; set; }

		public override string ToString()
		{
			return Width + "x" + Height;
		}
	}
}