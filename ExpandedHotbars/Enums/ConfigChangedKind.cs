using System;

namespace ExpandedHotbars.Enums;

/// <summary>
/// Enum representing what specific parts of the native ui are needing to be updated on a config change.
/// </summary>
[Flags]
public enum ConfigChangedKind : byte {
    None = 1 << 0,
    NeedsUpdate = 1 << 1,
    NeedsRebuild = 1 << 2,
}
