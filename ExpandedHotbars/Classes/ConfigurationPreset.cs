using ExpandedHotbars.Configuration;

namespace ExpandedHotbars.Classes;

public class ConfigurationPreset {
    public int Version { get; init; } = 1;
    public HotbarConfig? ConfigData { get; init; }
}
