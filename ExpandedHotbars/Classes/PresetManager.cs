using System;
using System.Linq;
using System.Text.Json;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ExpandedHotbars.Configuration;

namespace ExpandedHotbars.Classes;

public static class PresetManager {
    private static readonly JsonSerializerOptions SerializerOptions = new() {
        IncludeFields = true,
    };

    public static HotbarConfig? LoadFromClipboard() {
        try {
            var encodedString = ImGui.GetClipboardText();
            var decodedString = Convert.FromBase64String(encodedString);
            var uncompressed = Util.DecompressString(decodedString);

            if (string.IsNullOrEmpty(uncompressed)) {
                PrintError("Tried to import hotbar configuration, but got nothing, try copying the code again.");
                return null;
            }

            if (JsonSerializer.Deserialize<ConfigurationPreset>(uncompressed, SerializerOptions) is { } clipboardData) {
                if (clipboardData.ConfigData is null) {
                    PrintError("Tried to import hotbar configuration, but the data was invalid, try copying the code again.");
                    return null;
                }

                if (System.Config.Hotbars.Any(entry => entry.Guid == clipboardData.ConfigData.Guid)) {
                    PrintError("Tried to import already existing configuration, operation aborted.");
                    return null;
                }

                // Remove any invalid features of conditions, that may have been marked invalid after preset was made.
                clipboardData.ConfigData.ShowConditions.RemoveAll(condition => !condition.IsValid);
                clipboardData.ConfigData.ExtraFeatures.RemoveAll(feature => !feature.IsValid);

                IChatGui.Get().Print("[Import] Successfully imported hotbar configuration.", "ExpandedHotbars");
                System.Config.Hotbars.Add(clipboardData.ConfigData);
                System.Config.Save();

                return clipboardData.ConfigData;
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, "Error Parsing Preset");
            PrintError("Something went wrong trying to import hotbar configuration, check you copied the code correctly.");
        }

        return null;
    }

    public static void SaveToClipboard(HotbarConfig? config) {
        if (config is null) {
            PrintError("[Export] Attempted to save an unselected/invalid config, operation aborted.");
            return;
        }

        var presetContainer = new ConfigurationPreset {
            ConfigData = config,
        };

        var jsonString = JsonSerializer.Serialize(presetContainer, SerializerOptions);
        var compressedString = Util.CompressString(jsonString);
        var encodedString = Convert.ToBase64String(compressedString);

        IChatGui.Get().Print($"[Export] Exported hotbar '{config.HotbarName}' to clipboard.", "ExpandedHotbars");
        ImGui.SetClipboardText(encodedString);
    }

    private static void PrintError(string errorMessage)
        => IChatGui.Get().PrintError($"[Import] {errorMessage}", "ExpandedHotbars");
}
