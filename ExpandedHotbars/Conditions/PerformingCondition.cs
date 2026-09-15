using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class PerformingCondition : ConditionBase {
    public override string Name
        => "Performing";

    public override string Label
        => "Performing";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.Performing);
}
