using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class BoundByDutyCondition : ConditionBase {
    public override string Name
        => "Bound by Duty";

    public override string Label
        => "Bound by Duty";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.BoundByDuty, ConditionFlag.BoundByDuty56, ConditionFlag.BoundByDuty95);
}
