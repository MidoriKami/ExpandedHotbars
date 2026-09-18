using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Enums;

namespace ExpandedHotbars.Conditions;

public class RoleCondition : ConditionBase {
    public override string Name
        => "Role";

    public override string Label
        => Role is 0 ?
               "Role Not Selected" :
               $"Role ({Role.Label})";

    public ClassJobRole Role;

    protected override bool EvaluateCondition() {
        if (Role is 0) return false;
        if (!IClientState.Get().IsLoggedIn) return false;

        return IPlayerState.Get().ClassJob.Value.JobType == (uint)Role;
    }

    public override bool HasConfiguration
        => true;

    public override Vector2 ConfigSize
        => new(200.0f, 135.0f);

    public override void DrawConfig() {
        foreach (var option in Enum.GetValues<ClassJobRole>()) {
            if (ImGui.Selectable(option.Label, option == Role)) {
                Role = option;
                System.Config.Save();
            }
        }
    }
}
