using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class CrafterCondition : ConditionBase {
    public override string Name
        => "Crafter";

    public override string Label
        => "Crafter";

    protected override bool EvaluateCondition() {
        if (!IClientState.Get().IsLoggedIn) return false;

        return IPlayerState.Get().ClassJob.Value.ClassJobCategory.RowId is 32;
    }
}
