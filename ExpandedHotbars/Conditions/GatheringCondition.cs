using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class GatheringCondition : ConditionBase {
    public override string Name
        => "Gathering";

    public override string Label
        => "Gathering";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.Gathering);
}
