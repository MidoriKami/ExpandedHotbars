using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Conditions;
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Enums;
using ExpandedHotbars.Extensions;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Action = Lumina.Excel.Sheets.Action;

namespace ExpandedHotbars.Windows;

/// <summary>
/// Plugin configuration window, hopefully we can isolate *most* of the imgui monstosity in this file.
/// </summary>
public class ConfigWindow : Window {

    private HotbarConfig? selectedConfig;
    private ConditionBase? selectedCondition;
    private ConditionBase? configuringCondition;

    public ConfigWindow() : base("Expanded Hotbars Config Window") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(850.0f, 500.0f),
            MaximumSize = new Vector2(850.0f, 500.0f),
        };

        Flags |= ImGuiWindowFlags.NoResize;
    }

    public override void Draw() {
        using (var selectionChild = ImRaii.Child("SelectionChild", ImGui.RatioArea(0.3f, 1.0f))) {
            if (selectionChild) {
                DrawSelectionList();
            }
        }

        ImGui.SameLine();

        using (var configChild = ImRaii.Child("ConfigChild", ImGui.Area)) {
            if (configChild) {
                DrawConfigurationArea();
            }
        }
    }

    /// <summary>
    /// Draws the left hand panel of the ui, intended to show available hotbars, and a button to add a new one.
    /// Hi era~
    /// </summary>
    private void DrawSelectionList() {
        DrawSelectionListArea();
        DrawSelectionListButtons();
    }

    /// <summary>
    /// Draws the structural area for the list box contents, does not include add button.
    /// </summary>
    private void DrawSelectionListArea() {
        using var listArea = ImRaii.Child("ListArea", ImGui.RatioArea(1.0f, 0.95f));
        if (!listArea) return;

        DrawHotbarListBox();
    }

    /// <summary>
    /// Draws the listbox and its contents.
    /// </summary>
    private void DrawHotbarListBox() {
        using var listBox = ImRaii.ListBox("##HotbarSelectList", ImGui.Area);
        if (!listBox) return;

        using var itemSpacing = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 8.0f));

        foreach (var (index, option) in System.Config.Hotbars.Index()) {
            if (ImGui.Selectable($"\t{option.HotbarName}##{index}", selectedConfig == option)) {
                selectedConfig = option;
            }
        }
    }

    /// <summary>
    /// Draw buttons for selection list area.
    /// </summary>
    private static void DrawSelectionListButtons() {
        using var buttonArea = ImRaii.Child("ButtonArea", ImGui.Area);
        if (!buttonArea) return;

        if (ImGui.Button("Add", ImGui.Area)) {
            var newConfig = new HotbarConfig();
            System.Config.Hotbars.Add(newConfig);
            System.HotbarController.AddHotbar(newConfig);
            System.Config.Save();
        }
    }

    /// <summary>
    /// Draws the main config panel, including delete button unless I forget.
    /// </summary>
    private void DrawConfigurationArea() {
        if (selectedConfig is null) {
            ImGui.CenteredText(KnownColor.Orange.Vector(), "Select an option on the left\nAlternatively add a new hotbar", true);
            return;
        }

        ImGui.ScaledDummy(5.0f);
        ImGui.CenteredText(selectedConfig.HotbarName);
        ImGui.ScaledDummy(5.0f);

        using var tabBar = ImRaii.TabBar("TabBar");
        if (!tabBar) return;

        using (var configTab = ImRaii.TabItem("Hotbar")) {
            if (configTab) {
                DrawConfigTab();
            }
        }

        using (var extrasTab = ImRaii.TabItem("Conditions")) {
            if (extrasTab) {
                DrawConditionsTab();
            }
        }

        using (var keybindTab = ImRaii.TabItem("Keybinds")) {
            if (keybindTab) {
                DrawKeybindsTab();
            }
        }
    }

    private void DrawConfigTab() {
        if (selectedConfig is null) return;

        using var tabChild = ImRaii.Child("ConfigTab", ImGui.Area);
        if (!tabChild) return;

        using var itemSpacing = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 8.0f));

        ImGui.ScaledDummy(5.0f);

        ImGui.Label("Hotbar Name");
        ImGui.SetNextItemWidth(ImGui.AreaWidth);
        ImGui.InputTextWithHint("##HotbarName", "name", ref selectedConfig.HotbarName);

        if (ImGui.IsItemDeactivatedAfterEdit()) {
            System.Config.Save();
        }

        ImGui.Label("Hotbar Size");
        ImGui.SetNextItemWidth(ImGui.AreaWidth);
        ImGui.InputFloat2("##HotbarSize", ref selectedConfig.Size, 1.0f, 5.0f, "%.0f");
        selectedConfig.Size.X = Math.Clamp(selectedConfig.Size.X, 1.0f, 50.0f);
        selectedConfig.Size.Y = Math.Clamp(selectedConfig.Size.Y, 1.0f, 50.0f);

        if (ImGui.IsItemDeactivatedAfterEdit()) {
            selectedConfig.UpdateFlags |= ConfigChangedKind.NeedsRebuild;
            System.Config.Save();
        }

        ImGui.Label("Hotbar Slot Spacing");
        ImGui.SetNextItemWidth(ImGui.AreaWidth);
        ImGui.InputFloat2("##HotbarSpacing", ref selectedConfig.Spacing, 1.0f, 5.0f, "%.0f");

        if (ImGui.IsItemDeactivatedAfterEdit()) {
            selectedConfig.UpdateFlags |= ConfigChangedKind.NeedsUpdate;
            System.Config.Save();
        }

        ImGui.Label("Scale");
        ImGui.SetNextItemWidth(ImGui.AreaWidth);
        ImGui.SliderFloat("##Scale", ref selectedConfig.Scale, 0.5f, 5.0f);

        if (ImGui.IsItemDeactivatedAfterEdit()) {
            selectedConfig.UpdateFlags |= ConfigChangedKind.NeedsUpdate;
            System.Config.Save();
        }

        ImGui.Label("Enabled");
        if (ImGui.Checkbox("##Enabled", ref selectedConfig.IsEnabled)) {
            System.Config.Save();
        }

        ImGui.Label("Enable Moving");
        ImGui.Checkbox("##EnableMoving", ref selectedConfig.IsMovingEnabled);
        // Intentionally don't save this field, it's JsonIgnored.

        ImGui.Label("Enable Padlock Button");
        if (ImGui.Checkbox("##EnablePadlock", ref selectedConfig.ShowPadlockButton)) {
            selectedConfig.UpdateFlags |= ConfigChangedKind.NeedsUpdate;
            System.Config.Save();
        }

        ImGui.Label("Delete Hotbar");
        using (ImRaii.Disabled(!IKeyState.Get().DeleteKeybindPressed)) {
            if (ImGui.Button("Delete")) {
                System.Config.Hotbars.Remove(selectedConfig);
                System.HotbarController.RemoveHotbar(selectedConfig);
                selectedConfig = null;
                System.Config.Save();
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !IKeyState.Get().DeleteKeybindPressed) {
            ImGui.SetTooltip("Hold Control + Shift to enable button.");
        }
    }

    private void DrawConditionsTab() {
        if (selectedConfig is null) return;

        using var tabChild = ImRaii.Child("ConditionsTab", ImGui.Area);
        if (!tabChild) return;

        ImGui.ScaledDummy(5.0f);

        ImGui.Text("Condition Mode");
        ImGui.SameLine(ImGui.TotalWidth / 3.0f);

        if (ImGui.RadioButton("Show When All", selectedConfig.RequireAllConditions)) {
            selectedConfig.RequireAllConditions = true;
            System.Config.Save();
        }

        ImGui.SameLine(ImGui.TotalWidth * 2.0f / 3.0f);

        if (ImGui.RadioButton("Show When Any", !selectedConfig.RequireAllConditions)) {
            selectedConfig.RequireAllConditions = false;
            System.Config.Save();
        }

        ImGui.ScaledDummy(5.0f);
        ImGui.Separator();

        if (ImGui.Button("Add Condition", ImGui.ScaledVector(250.0f, 22.0f))) {
            if (selectedCondition is not null) {
                if (Activator.CreateInstance(selectedCondition.GetType()) is ConditionBase newCondition) {
                    selectedConfig.ShowConditions.Add(newCondition);
                    System.Config.Save();
                }
            }
        }

        ImGui.SameLine();

        ImGui.SetNextItemWidth(ImGui.AreaWidth);
        using (var dropdown = ImRaii.Combo("##ConditionSelect", selectedCondition?.Name ?? "Select a Condition", ImGuiComboFlags.HeightLarge)) {
            if (dropdown) {
                foreach (var option in System.GetConditions().OrderBy(condition => condition.Name)) {
                    if (ImGui.Selectable(option.Name, selectedCondition == option)) {
                        selectedCondition = option;
                    }
                }
            }
        }

        ImGui.ScaledDummy(5.0f);

        if (selectedConfig.ShowConditions.Count is 0) {
            ImGui.CenteredText(KnownColor.Orange.Vector(), "No Conditions Defined", true);
        }
        else {
            var buttonSize = new Vector2(ImGui.TotalWidth / 6.0f - ImGui.ItemSpacing.X, ImGui.Scaled(22.0f));
            ConditionBase? removalOption = null;

            using var conditionsChild = ImRaii.Child("ConditionOptions", ImGui.Area);
            if (conditionsChild) {
                foreach (var (index, condition) in selectedConfig.ShowConditions.Index()) {
                    if (ImGui.Button(condition.Invert ? $"Disallowed##{index}" : $"Allowed##{index}", new Vector2(ImGui.TotalWidth / 6.0f, ImGui.Scaled(22.0f)))) {
                        condition.Invert = !condition.Invert;
                        System.Config.Save();
                    }

                    ImGui.SameLine(ImGui.TotalWidth / 6.0f + ImGui.ItemSpacing.X);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text(condition.Label);

                    ImGui.SameLine(ImGui.TotalWidth * 2.0f / 3.0f + ImGui.ItemSpacing.X);
                    using (ImRaii.Disabled(!condition.HasConfiguration)) {
                        if (ImGui.Button($"Configure##{index}", buttonSize)) {
                            ImGui.OpenPopup("ConditionConfigPopup");
                            configuringCondition = condition;
                        }
                    }
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !condition.HasConfiguration) {
                        ImGui.SetTooltip("This option is not configurable.");
                    }

                    ImGui.SameLine(ImGui.TotalWidth * 5.0f / 6.0f + ImGui.ItemSpacing.X);

                    using (ImRaii.Disabled(!IKeyState.Get().DeleteKeybindPressed)) {
                        if (ImGui.Button($"Delete##{index}", buttonSize)) {
                            removalOption = condition;
                        }
                    }

                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !IKeyState.Get().DeleteKeybindPressed) {
                        ImGui.SetTooltip("Hold Control + Shift to enable button.");
                    }

                    ImGui.ScaledDummy(0.0f);
                }

                if (removalOption is { } option) {
                    selectedConfig.ShowConditions.Remove(option);
                }
            }

            DrawConditionConfigPopup();
        }
    }

    private void DrawConditionConfigPopup() {
        if (configuringCondition is null) return;

        ImGui.SetNextWindowSize(configuringCondition.ConfigSize);

        using var popup = ImRaii.Popup("ConditionConfigPopup");
        if (!popup) {
            configuringCondition = null;
            return;
        }

        configuringCondition.DrawConfig();
    }

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
