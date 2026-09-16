using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class CutsceneCondition : ConditionBase {
    public override string Name
        => "Cutscene";

    public override string Label
        => "Cutscene";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.OccupiedInCutSceneEvent);
}
