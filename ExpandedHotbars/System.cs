using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Windowing;
using ExpandedHotbars.Classes;
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Windows;

namespace ExpandedHotbars;

public static class System {
    public static SystemConfiguration Config { get; set; } = null!;
    public static HotbarController HotbarController { get; set; } = null!;
    public static ConfigWindow ConfigWindow { get; set; } = null!;
    public static WindowSystem WindowSystem { get; set; } = null!;
    public static KeybindWindow KeybindWindow { get; set; } = null!;
    public static IFontHandle MeidingerMidFont { get; set; } = null!;
}
