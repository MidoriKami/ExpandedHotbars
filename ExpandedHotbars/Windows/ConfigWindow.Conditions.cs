using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Conditions;
using ExpandedHotbars.Extensions;

namespace ExpandedHotbars.Windows;

public partial class ConfigWindow {
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
}
