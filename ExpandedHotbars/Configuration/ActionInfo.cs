using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ExpandedHotbars.Configuration;

/// <summary>
/// Data object represneting a hotbar action and its locations
/// </summary>
public class ActionInfo {
    /// <summary>
    /// The data type for this slot
    /// </summary>
    public DragDropType DragDropType;

    /// <summary>
    /// The slot's command ID
    /// </summary>
    public uint ActionId;
}
