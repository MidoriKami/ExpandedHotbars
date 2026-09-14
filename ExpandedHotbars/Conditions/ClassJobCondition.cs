using System.Linq;
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

    public override string Label
        => "ClassJob";

    public uint ClassJob;

    public override bool IsConditionMet() {
        var currentJob = IPlayerState.Get().ClassJob.RowId;

        if (currentJob is 0) return true;

        return currentJob == ClassJob;
    }

    public override void DrawConfig() {
        using var popup = ImRaii.Popup(Label);
        if (!popup) return;

        var classJobs = IDataManager.Get().GetExcelSheet<ClassJob>()
            .Where(job => job.ClassJobCategory.RowId is not 0)
            .OrderBy(job => job.UIPriority);

        foreach (var option in classJobs) {
            if (ImGui.Selectable(option.Name.ToString(), ClassJob == option.RowId)) {
                ClassJob = option.RowId;
            }
        }
    }
}
