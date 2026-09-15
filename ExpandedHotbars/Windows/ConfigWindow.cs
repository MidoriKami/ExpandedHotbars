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
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Enums;
using ExpandedHotbars.Extensions;
using Action = Lumina.Excel.Sheets.Action;

namespace ExpandedHotbars.Windows;

/// <summary>
/// Plugin configuration window, hopefully we can isolate *most* of the imgui monstosity in this file.
/// </summary>
public class ConfigWindow : Window {

    private HotbarConfig? selectedConfig;

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
            System.Config.Hotbars.Add(new HotbarConfig());
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

    }

    private void DrawKeybindsTab() {
        if (selectedConfig is null) return;

        using var tabChild = ImRaii.Child("KeybindsTab", ImGui.Area);
        if (!tabChild) return;

        ImGui.ScaledDummy(5.0f);

        foreach (var row in Enumerable.Range(0, (int) selectedConfig.Size.Y)) {
            foreach (var column in Enumerable.Range(0, (int) selectedConfig.Size.X)) {
                using var id = ImRaii.PushId($"{row},{column}");

                var iconId = 0U;
                var actionName = string.Empty;

                if (selectedConfig.Actions.TryGetValue((row, column), out var actionInfo)) {
                    var actionData = IDataManager.Get().GetExcelSheet<Action>().GetRow(actionInfo.ActionId);

                    iconId = actionData.Icon;
                    actionName = actionData.Name.ToString();
                }

                ImGui.Image(ITextureProvider.Get().GetFromGameIcon(iconId).GetWrapOrEmpty().Handle, new Vector2(24.0f, 24.0f));

                ImGui.SameLine(ImGui.Scaled(50.0f));
                ImGui.AlignTextToFramePadding();
                ImGui.Text(actionName);

                ImGui.SameLine(ImGui.Scaled(200.0f));
                ImGui.AlignTextToFramePadding();
                ImGui.Text($"Row {row + 1} Column {column + 1}");

                selectedConfig.Keybinds.TryGetValue((row, column), out var keybindInfo);

                ImGui.SameLine(ImGui.Scaled(350.0f));

                using var font = System.MeidingerMidFont.Push();

                if (ImGui.Button(keybindInfo?.ToString() ?? "", new Vector2(ImGui.AreaWidth, 24.0f))) {
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
        }
    }
}
