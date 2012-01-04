namespace Travlexer.WindowsPhone.Services.GoogleMaps
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