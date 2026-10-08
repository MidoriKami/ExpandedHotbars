using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ExpandedHotbars.Conditions;
using ExpandedHotbars.Configuration;
using ExpandedHotbars.Extensions;

namespace ExpandedHotbars.Windows;

/// <summary>
/// Plugin configuration window, hopefully we can isolate *most* of the imgui monstosity in this file.
/// </summary>
public partial class ConfigWindow : Window {

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

        using (var conditionsTab = ImRaii.TabItem("Conditions")) {
            if (conditionsTab) {
                DrawConditionsTab();
            }
        }

        using (var extrasTab = ImRaii.TabItem("Extras")) {
            if (extrasTab) {
                DrawExtrasTab();
            }
        }

        using (var keybindTab = ImRaii.TabItem("Keybinds")) {
            if (keybindTab) {
                DrawKeybindsTab();
            }
        }
    }
}
