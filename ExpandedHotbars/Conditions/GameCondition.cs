using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace ExpandedHotbars.Conditions;

public class GameCondition : ConditionBase {
    public override string Name
        => "Generic Condition";

    public override string Label
        => Condition is 0 ?
               "Condition Not Set" :
               $"Condition ({Condition})";

    public ConditionFlag Condition;

    protected override bool EvaluateCondition()
        => ICondition.Get().Any(Condition);

    public override bool HasConfiguration
        => true;

    public override Vector2 ConfigSize
        => new(250.0f, 550.0f);

    public override void DrawConfig() {
        foreach (var condition in Enum.GetValues<ConditionFlag>().Skip(1).Distinct()) {
            if (ImGui.Selectable(condition.ToString(), Condition == condition)) {
                Condition = condition;
                System.Config.Save();
            }
        }
    }
}
