using Dalamud.Game.ClientState.Keys;
using FFXIVClientStructs.FFXIV.Client.System.Input;

namespace ExpandedHotbars.Configuration;

/// <summary>
/// Data object representing a keybind.
/// </summary>
public class KeybindInfo {
    public VirtualKey Key = VirtualKey.NO_KEY;
    public VirtualKey Modifier = VirtualKey.NO_KEY;

    public override string ToString() {
        var modifierKey = Modifier switch {
            VirtualKey.MENU => "ª",
            VirtualKey.SHIFT => "§",
            VirtualKey.CONTROL => "¢",
            _ => null,
        };

        return $"{modifierKey}{(char)Key}";
    }

    public static implicit operator KeySetting(KeybindInfo keybindInfo) => new() {
        Key = (SeVirtualKey)keybindInfo.Key,
        KeyModifier = keybindInfo.GetKeyModifier(),
    };

    private KeyModifierFlag GetKeyModifier() {
        if (Modifier is VirtualKey.MENU) return KeyModifierFlag.Alt;
        if (Modifier is VirtualKey.SHIFT) return KeyModifierFlag.Shift;
        if (Modifier is VirtualKey.CONTROL) return KeyModifierFlag.Ctrl;
        return KeyModifierFlag.None;
    }
}
