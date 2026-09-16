using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace ExpandedHotbars.Conditions;

public class DutyTypeCondition : ConditionBase {
    public override string Name
        => "Duty Type";

    public override string Label
        => ContentType is 0 ?
               "Duty Type Not Selected" :
               $"Duty Type ({IDataManager.Get().GetExcelSheet<ContentType>().GetRow(ContentType).Name})";

    public uint ContentType;

    protected override unsafe bool EvaluateCondition()
        => IDataManager.Get()
               .GetExcelSheet<ContentFinderCondition>()
               .GetRow(GameMain.Instance()->CurrentContentFinderConditionId)
               .ContentType.RowId == ContentType;

    public override bool HasConfiguration
        => true;

    public override Vector2 ConfigSize
        => new(350.0f, 732.0f);

    public override void DrawConfig() {
        var contentTypes = IDataManager.Get().GetExcelSheet<ContentType>()
            .Where(type => !type.Name.IsEmpty);

        foreach (var contentType in contentTypes) {
            if (ImGui.Selectable(contentType.Name.ToString(), contentType.RowId == ContentType)) {
                ContentType = contentType.RowId;
                System.Config.Save();
            }
        }
    }
}
