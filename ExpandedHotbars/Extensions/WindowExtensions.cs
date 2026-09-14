using System.Diagnostics;
using Dalamud.Interface.Windowing;

namespace ExpandedHotbars.Extensions;

public static class WindowExtensions {
    extension(Window window) {
        [Conditional("DEBUG")]
        public void DebugToggle()
            => window.Toggle();
    }
}
