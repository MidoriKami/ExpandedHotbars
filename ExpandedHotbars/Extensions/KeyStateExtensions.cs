using System.Collections.Generic;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Extensions;

public static class KeyStateExtensions {
    extension(IKeyState keyState) {
        public bool DeleteKeybindPressed
            => keyState.IsVirtualKeyValid(VirtualKey.CONTROL) && keyState[VirtualKey.CONTROL] &&
               keyState.IsVirtualKeyValid(VirtualKey.SHIFT) && keyState[VirtualKey.SHIFT];

        public bool IsKeybindPressed(IEnumerable<VirtualKey> keys) {
            foreach (var key in keys) {
                if (keyState.IsVirtualKeyValid(key) && !keyState[(int)key]) {
                    return false;
                }
            }

            return true;
        }

        public void ResetKeyCombo(IEnumerable<VirtualKey> keys) {
            foreach (var key in keys) {
                if (!keyState.IsVirtualKeyValid(key)) continue;
                if (key is VirtualKey.CONTROL or VirtualKey.MENU or VirtualKey.SHIFT) continue;

                keyState[(int)key] = false;
            }
        }
    }
}
