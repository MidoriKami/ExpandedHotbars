using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class GameEventCondition : ConditionBase {
    public override string Name
        => "Game Event";

    public override string Label
        => "Game Event";

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(ConditionFlag.OccupiedInQuestEvent, ConditionFlag.OccupiedInCutSceneEvent, ConditionFlag.OccupiedInEvent);
}
