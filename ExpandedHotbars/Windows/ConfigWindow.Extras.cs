using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Extensions;
using ExpandedHotbars.Extras;

namespace ExpandedHotbars.Windows;

public partial class ConfigWindow {
    private ExtrasBase? selectedExtra;
    private ExtrasBase? configuringExtra;

    private void DrawExtrasTab() {
        if (selectedConfig is null) return;

        using var tabChild = ImRaii.Child("ExtrasTab", ImGui.Area);
        if (!tabChild) return;

        ImGui.ScaledDummy(5.0f);

        ImGui.Text("When this hotbar is active perform the following action(s)");
        ImGui.ScaledDummy(5.0f);

        ImGui.Separator();

        if (ImGui.Button("Add Feature", ImGui.ScaledVector(250.0f, 22.0f))) {
            if (selectedExtra is not null) {
                if (Activator.CreateInstance(selectedExtra.GetType()) is ExtrasBase newFeature) {
                    newFeature.OnEnable();
                    selectedConfig.ExtraFeatures.Add(newFeature);
                    System.Config.Save();
                }
            }
        }

        ImGui.SameLine();

        ImGui.SetNextItemWidth(ImGui.AreaWidth);
        using (var dropdown = ImRaii.Combo("##FeatureSelect", selectedExtra?.Name ?? "Select a Feature", ImGuiComboFlags.HeightLarge)) {
            if (dropdown) {
                foreach (var option in System.GetExtras().OrderBy(condition => condition.Name)) {
                    if (ImGui.Selectable(option.Name, selectedExtra == option)) {
                        selectedExtra = option;
                    }
                }
            }
        }

        ImGui.ScaledDummy(5.0f);

        if (selectedConfig.ExtraFeatures.Count is 0) {
            ImGui.CenteredText(KnownColor.Orange.Vector(), "No Extra Features Defined", true);
        }
        else {
            var buttonSize = new Vector2(ImGui.TotalWidth / 6.0f - ImGui.ItemSpacing.X, ImGui.Scaled(22.0f));
            ExtrasBase? removalOption = null;

            using var extrasChild = ImRaii.Child("ExtraOptions", ImGui.Area);
            if (extrasChild) {
                foreach (var (index, extraFeature) in selectedConfig.ExtraFeatures.Index()) {
                    if (ImGui.Button(extraFeature.Enabled ? $"Enabled##{index}" : $"Disabled##{index}", new Vector2(ImGui.TotalWidth / 6.0f, ImGui.Scaled(22.0f)))) {
                        extraFeature.Enabled = !extraFeature.Enabled;

                        if (extraFeature.Enabled) {
                            extraFeature.OnEnable();
                        }
                        else {
                            extraFeature.OnDisable();
                        }
                        System.Config.Save();
                    }

                    ImGui.SameLine(ImGui.TotalWidth / 6.0f + ImGui.ItemSpacing.X);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text(extraFeature.Label);

                    ImGui.SameLine(ImGui.TotalWidth * 2.0f / 3.0f + ImGui.ItemSpacing.X);
                    using (ImRaii.Disabled(!extraFeature.HasConfiguration)) {
                        if (ImGui.Button($"Configure##{index}", buttonSize)) {
                            ImGui.OpenPopup("ExtrasConfigPopup");
                            configuringExtra = extraFeature;
                        }
                    }
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !extraFeature.HasConfiguration) {
                        ImGui.SetTooltip("This option is not configurable.");
                    }

                    ImGui.SameLine(ImGui.TotalWidth * 5.0f / 6.0f + ImGui.ItemSpacing.X);

                    using (ImRaii.Disabled(!IKeyState.Get().DeleteKeybindPressed)) {
                        if (ImGui.Button($"Delete##{index}", buttonSize)) {
                            removalOption = extraFeature;
                        }
                    }

                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !IKeyState.Get().DeleteKeybindPressed) {
                        ImGui.SetTooltip("Hold Control + Shift to enable button.");
                    }

                    ImGui.ScaledDummy(0.0f);
                }

                if (removalOption is { } option) {
                    option.OnDisable();
                    selectedConfig.ExtraFeatures.Remove(option);
                }
            }

            DrawExtrasConfigPopup();
        }
    }

    private void DrawExtrasConfigPopup() {
        if (configuringExtra is null) return;

        ImGui.SetNextWindowSize(configuringExtra.ConfigSize);

        using var popup = ImRaii.Popup("ExtrasConfigPopup");
        if (!popup) {
            configuringExtra = null;
            return;
        }

        configuringExtra.DrawConfig();
    }
}
