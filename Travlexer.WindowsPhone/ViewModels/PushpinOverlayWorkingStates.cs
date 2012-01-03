using Travlexer.WindowsPhone.Controls;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the working states of a <see cref="PushpinContent"/>.
	/// </summary>
	public enum PushpinOverlayWorkingStates : byte
	{
		Idle = 0,
		Working,
		Error
	}
}