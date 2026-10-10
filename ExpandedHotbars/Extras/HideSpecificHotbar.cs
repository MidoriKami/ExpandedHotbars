using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ExpandedHotbars.Extras;

public class HideSpecificHotbar : ExtrasBase {

    private readonly List<string> allAddonNames = [
        "_ActionBarDoubleCrossR",
        "_ActionBarDoubleCrossL",
        "_ActionCross",
        "_ActionBar",
        "_ActionBar01",
        "_ActionBar02",
        "_ActionBar03",
        "_ActionBar04",
        "_ActionBar05",
        "_ActionBar06",
        "_ActionBar07",
        "_ActionBar08",
        "_ActionBar09",
    ];

    public override string Name
        => "Hide Specific Hotbar";

    public override string Label
        => $"Hide Specific Hotbar ({HiddenHotbar ?? "Unselected"})";

    public string? HiddenHotbar;

    public override bool HasConfiguration
        => true;

    public override Vector2 ConfigSize { get; }
        = new(200.0f, 350.0f);

    public override void DrawConfig() {
        foreach (var hotbar in allAddonNames) {
            if (ImGui.Selectable(hotbar, hotbar == HiddenHotbar)) {
                HiddenHotbar = hotbar;
                System.Config.Save();
                ImGui.CloseCurrentPopup();
            }
        }
    }

    public override unsafe void Update() {
        if (HiddenHotbar is null) return;

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(HiddenHotbar);
        if (addon is not null) {
            addon->RootNode->ToggleVisibility(false);
        }
    }

    public override unsafe void OnDisable() {
        base.OnDisable();

        if (HiddenHotbar is null) return;

        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(HiddenHotbar);
        if (addon is not null) {
            addon->RootNode->ToggleVisibility(addon->IsVisible);
        }
    }
}
