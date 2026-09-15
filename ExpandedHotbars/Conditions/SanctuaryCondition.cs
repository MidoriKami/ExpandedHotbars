using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace ExpandedHotbars.Conditions;

public class InSanctuaryCondition : ConditionBase {
    public override string Name
        => "Sanctuary";

    public override string Label
        => "Sanctuary";

    protected override unsafe bool EvaluateCondition()
        => TerritoryInfo.Instance()->InSanctuary;
}
