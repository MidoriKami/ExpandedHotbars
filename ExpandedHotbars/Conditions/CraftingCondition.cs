using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class CraftingCondition : ConditionBase {
    public override string Name
        => "Crafting";

    public override string Label
        => "Crafting";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.Crafting);
}
