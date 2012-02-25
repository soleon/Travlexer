namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// A simple class that holds 3 fields representing the key, the value and the icon.
	/// </summary>
	public class KeyValueIcon<TKey, TValue, TIcon>
	{
		public TKey Key { get; set; }

		public TValue Value { get; set; }

		public TIcon Icon { get; set; }
	}
}