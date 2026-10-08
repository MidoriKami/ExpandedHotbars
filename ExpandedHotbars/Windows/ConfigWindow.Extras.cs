using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using ExpandedHotbars.Extensions;

namespace ExpandedHotbars.Windows;

public partial class ConfigWindow {
    private void DrawExtrasTab() {
        if (selectedConfig is null) return;

        using var tabChild = ImRaii.Child("ExtrasTab", ImGui.Area);
        if (!tabChild) return;

        ImGui.ScaledDummy(5.0f);

        ImGui.Text("Work in Progress, please look forwards to it!");
    }
}
