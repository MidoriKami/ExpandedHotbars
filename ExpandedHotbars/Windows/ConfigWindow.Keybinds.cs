using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Enums;
using ExpandedHotbars.Extensions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace ExpandedHotbars.Windows;

public partial class ConfigWindow {
    private void DrawKeybindsTab() {
        if (selectedConfig is null) return;

        using var tabChild = ImRaii.Child("KeybindsTab", ImGui.Area);
        if (!tabChild) return;

        ImGui.ScaledDummy(5.0f);

        foreach (var row in Enumerable.Range(0, (int) selectedConfig.Size.Y)) {
            foreach (var column in Enumerable.Range(0, (int) selectedConfig.Size.X)) {
                using var id = ImRaii.PushId($"{row},{column}");

                ImGui.AlignTextToFramePadding();
                ImGui.Text($"Row {row + 1} Column {column + 1}");
                ImGui.SameLine(ImGui.Scaled(150.0f));

                selectedConfig.Keybinds.TryGetValue((row, column), out var keybindInfo);

                using (System.MeidingerMidFont.Push()) {
                    if (ImGui.Button(keybindInfo?.ToString() ?? "", ImGui.ScaledVector(100.0f, 24.0f))) {
                        System.KeybindWindow.KeybindConfirmed = keyCombo => {
                            if (keybindInfo is null) {
                                selectedConfig.Keybinds.TryAdd((row, column), keyCombo);
                            }
                            else {
                                keybindInfo.Key = keyCombo.Key;
                                keybindInfo.Modifier = keyCombo.Modifier;
                            }

                            selectedConfig.UpdateFlags |= ConfigChangedKind.NeedsUpdate;
                            System.Config.Save();
                        };

                        System.KeybindWindow.KeybindCleared = () => {
                            keybindInfo?.Key = VirtualKey.NO_KEY;
                            keybindInfo?.Modifier = VirtualKey.NO_KEY;

                            selectedConfig.UpdateFlags |= ConfigChangedKind.NeedsUpdate;
                            System.Config.Save();
                        };

                        System.KeybindWindow.IsOpen = true;
                    }
                }

                var iconId = 0U;
                var actionName = string.Empty;

                if (selectedConfig.Actions.TryGetValue((row, column), out var actionInfo)) {
                    (iconId, actionName) = GetDrawInfo(actionInfo);
                }

                ImGui.SameLine(ImGui.Scaled(275.0f));
                ImGui.Image(ITextureProvider.Get().GetFromGameIcon(iconId).GetWrapOrEmpty().Handle, new Vector2(24.0f, 24.0f));

                ImGui.SameLine(ImGui.Scaled(325.0f));
                ImGui.AlignTextToFramePadding();
                ImGui.Text(actionName);
            }
        }
    }

    private static unsafe (uint icon, string name) GetDrawInfo(ActionInfo info) {
        switch (info.DragDropType) {
            case DragDropType.Action:
                var actionData = IDataManager.Get().GetExcelSheet<Action>().GetRow(info.ActionId);
                return (actionData.Icon, actionData.Name.ToString());

            case DragDropType.GeneralAction:
                var generalActionData = IDataManager.Get().GetExcelSheet<GeneralAction>().GetRow(info.ActionId);
                return ((uint)generalActionData.Icon, generalActionData.Name.ToString());

            case DragDropType.Macro:
                var macroData = RaptureMacroModule.Instance()->GetMacro(info.ActionId / 0x100, info.ActionId % 0x100);
                return (macroData->IconId, macroData->Name.ToString());

            case DragDropType.Item:
                var sorterEntry = ItemOrderModule.Instance()->InventorySorter->Items[info.ReferenceId].Value;
                var item = InventoryManager.Instance()->GetInventorySlot((InventoryType) sorterEntry->Page, sorterEntry->Slot);
                var iconId = item->IconId;
                var itemName = item->Name;
                return (iconId, itemName.ToString());

            default:
                if (info.ActionId is not (0 or uint.MaxValue)) {
                    return (60861, $"Unable to Parse Type '{info.DragDropType}'");
                }
                else {
                    return (0U, string.Empty);
                }
        }
    }
}
