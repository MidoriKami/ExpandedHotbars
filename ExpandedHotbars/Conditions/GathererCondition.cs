using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class GathererCondition : ConditionBase {
    public override string Name
        => "Gatherer";

    public override string Label
        => "Gatherer";

    protected override bool EvaluateCondition()
        => IPlayerState.Get().ClassJob.Value.ClassJobCategory.RowId is 33;
}
