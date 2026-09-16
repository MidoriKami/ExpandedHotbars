using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class MountedCondition : ConditionBase {
    public override string Name
        => "Mounted";

    public override string Label
        => "Mounted";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.Mounted);
}
