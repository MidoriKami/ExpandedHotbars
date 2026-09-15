using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace ExpandedHotbars.Conditions;

/// <summary>
/// Condition representing the player being on a very specific ClassJob.
/// Not intended to represent multiple jobs.
/// </summary>
public class ClassJobCondition : ConditionBase {
    public override string Name
        => "ClassJob";

    public override string Label
        => ClassJob is 0 ?
               "ClassJob (Job not Selected)" :
               $"ClassJob ({ISeStringEvaluator.Get().EvaluateFromAddon(698, [ClassJob])})";

    public uint ClassJob;

    public override bool HasConfiguration
        => true;

    public override Vector2 ConfigSize
        => new(200.0f, 500.0f);

    protected override bool EvaluateCondition() {
        var currentJob = IPlayerState.Get().ClassJob.RowId;

        if (currentJob is 0) return true;

        return currentJob == ClassJob;
    }

    public override void DrawConfig() {
        var classJobs = IDataManager.Get().GetExcelSheet<ClassJob>()
            .Where(job => !job.Name.IsEmpty)
            .OrderBy(job => job.UIPriority);

        foreach (var option in classJobs) {
            using var id = ImRaii.PushId(option.RowId.ToString());

            var label = ISeStringEvaluator.Get().EvaluateFromAddon(698, [option.RowId]).ToString();
            if (ImGui.Selectable(label, ClassJob == option.RowId)) {
                ClassJob = option.RowId;
            }
        }
    }
}
