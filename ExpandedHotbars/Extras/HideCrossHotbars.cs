using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ExpandedHotbars.Extras;

public class HideCrossHotbars : ExtrasBase {

    private readonly List<string> crossHotbarAddonNames = [
        "_ActionBarDoubleCrossR",
        "_ActionBarDoubleCrossL",
        "_ActionCross",
    ];

    public override string Name
        => "Hide Cross Hotbars";

    public override string Label
        => "Hide Cross Hotbars";

    public override unsafe void Update() {
        foreach (var hotbar in crossHotbarAddonNames) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(hotbar);
            if (addon != null) {
                addon->RootNode->ToggleVisibility(false);
            }
        }
    }

    public override unsafe void OnDisable() {
        base.OnDisable();

        foreach (var hotbar in crossHotbarAddonNames) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(hotbar);
            if (addon != null) {
                addon->RootNode->ToggleVisibility(addon->IsVisible);
            }
        }
    }
}
