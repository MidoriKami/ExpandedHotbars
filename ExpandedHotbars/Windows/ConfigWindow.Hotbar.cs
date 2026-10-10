using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using ExpandedHotbars.Enums;
using ExpandedHotbars.Extensions;

namespace ExpandedHotbars.Windows;

public partial class ConfigWindow {
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
            foreach (var feature in selectedConfig.ExtraFeatures) {
                if (selectedConfig.IsEnabled) {
                    feature.OnEnable();
                }
                else {
                    feature.OnDisable();
                }
            }

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

        ImGui.Label("Allow Clicking Actions", "When checked allows invoking actions via mouse clicks.");
        if (ImGui.Checkbox("##EnableClicking", ref selectedConfig.EnableClicking)) {
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
}
