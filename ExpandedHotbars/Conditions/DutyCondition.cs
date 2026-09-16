using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Extensions;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace ExpandedHotbars.Conditions;

public class DutyCondition : ConditionBase {
    public override string Name
        => "Specific Duty";

    public override string Label
        => Duty is 0 ?
               "Duty not Selected" :
               ISeStringEvaluator.Get().EvaluateFromAddon(9781, [Duty]).ToString();

    public uint Duty;

    protected override unsafe bool EvaluateCondition()
        => GameMain.Instance()->CurrentContentFinderConditionId == Duty;

    public override bool HasConfiguration
        => true;

    public override Vector2 ConfigSize
        => new(350.0f, 500.0f);

    public override void DrawConfig() {
        searchResults ??= searchOptions;

        ImGui.SetNextItemWidth(ImGui.AreaWidth);

        if (ImGui.IsWindowAppearing()) {
            ImGui.SetKeyboardFocusHere();
        }

        if (ImGui.InputTextWithHint("##Search", "Search...", ref searchString, flags: ImGuiInputTextFlags.AutoSelectAll)) {
            try {
                var searchRegex = new Regex(searchString, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                searchResults = [
                    .. searchOptions.Where(option => searchRegex.IsMatch(option.Name.ToString())),
                ];
            }
            catch {
                searchResults = searchOptions;
            }
        }

        ImGui.Separator();

        using var child = ImRaii.Child("ResultsChild", ImGui.Area);
        if (!child) return;

        if (searchResults.Count is 0) {
            ImGui.CenteredText(KnownColor.Orange.Vector(), "No Results");
        }
        else {
            foreach (var result in searchResults) {
                var label = ISeStringEvaluator.Get().EvaluateFromAddon(9781, [result.RowId]).ToString();
                if (ImGui.Selectable(label, Duty == result.RowId)) {
                    Duty = result.RowId;
                    System.Config.Save();
                    ImGui.CloseCurrentPopup();
                }
            }
        }
    }

    private string searchString = string.Empty;

    private List<ContentFinderCondition>? searchResults;

    private readonly List<ContentFinderCondition> searchOptions = [
        .. IDataManager.Get().GetExcelSheet<ContentFinderCondition>().Where(cfc => !cfc.Name.IsEmpty),
    ];
}
