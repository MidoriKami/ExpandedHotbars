using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ExpandedHotbars.Extras;

public class HideStandardHotbars : ExtrasBase {

    private readonly List<string> standardHotbarAddonNames = [
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
        => "Hide Standard Hotbars";

    public override string Label
        => "Hide Standard Hotbars";

    public override unsafe void Update() {
        foreach (var hotbar in standardHotbarAddonNames) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(hotbar);
            if (addon != null) {
                addon->RootNode->ToggleVisibility(false);
            }
        }
    }

    public override unsafe void OnDisable() {
        base.OnDisable();

        foreach (var hotbar in standardHotbarAddonNames) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(hotbar);
            if (addon != null) {
                addon->RootNode->ToggleVisibility(addon->IsVisible);
            }
        }
    }
}
