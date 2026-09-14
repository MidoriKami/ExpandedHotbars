using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Extensions;

public static class KeyStateExtensions {
	extension(IKeyState keyState) {
		public bool DeleteKeybindPressed
			=> keyState.IsVirtualKeyValid(VirtualKey.CONTROL) && keyState[VirtualKey.CONTROL] &&
			   keyState.IsVirtualKeyValid(VirtualKey.SHIFT) && keyState[VirtualKey.SHIFT];
	}
}
