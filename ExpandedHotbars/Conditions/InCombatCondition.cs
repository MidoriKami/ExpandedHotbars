using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class InCombatCondition : ConditionBase {
    public override string Name
        => "In Combat";

    public override string Label
        => "In Combat";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.InCombat);
}
