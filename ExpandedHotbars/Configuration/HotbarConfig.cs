using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using ExpandedHotbars.Conditions;
using ExpandedHotbars.Enums;

namespace ExpandedHotbars.Configuration;

/// <summary>
/// Configuration file representing a single hotbar.
/// </summary>
public class HotbarConfig {

    /// <summary>
    /// Name of this hotbar, names don't have to be unique, but commands may fail if they aren't unique.
    /// </summary>
    public string HotbarName = "New Hotbar";

    /// <summary>
    /// Slot dimensions, ie 12x1 would be a normal default vanilla hotbar
    /// </summary>
    public Vector2 Size = new(12, 1);

    /// <summary>
    /// Scale as in zoom level for the hotbar
    /// </summary>
    public float Scale = 1.0f;

    /// <summary>
    /// Spacing between slots. Default value tries to be close to vanilla.
    /// </summary>
    public Vector2 Spacing = new(0.0f, 4.0f);

    /// <summary>
    /// The current position of the hotbar, ideally this is spawned in the middle of the screen,
    /// and then saved/updated when moved.
    /// </summary>
    public Vector2 Position = Vector2.Zero;

    /// <summary>
    /// If this hotbar should be shown if the conditions are met.
    /// </summary>
    public bool IsEnabled = true;

    /// <summary>
    /// If this hotbar should include and show the padlock button.
    /// </summary>
    public bool ShowPadlockButton = true;

    /// <summary>
    /// List of conditions that must be met to enable showing this hotbar.
    /// </summary>
    public List<ConditionBase> ShowConditions = [];

    /// <summary>
    /// When true, all conditions in ShowConditions must be met, when false, only one must be met to show.
    /// </summary>
    public bool RequireAllConditions = true;

    /// <summary>
    /// Dictionary of actions this hotbar holds.
    /// </summary>
    public Dictionary<HotbarLocation, ActionInfo> Actions = [];

    /// <summary>
    /// Dictionary of keybinds this hotbar hold.
    /// </summary>
    public Dictionary<HotbarLocation, KeybindInfo> Keybinds = [];

    //
    // Non Serialized Properties used during configuration
    //

    /// <summary>
    /// Flags that indicate what part of the native hotbar to update on a config change.
    /// </summary>
    [JsonIgnore] public ConfigChangedKind UpdateFlags = ConfigChangedKind.NeedsRebuild | ConfigChangedKind.NeedsUpdate;

    /// <summary>
    /// Indicates if the hotbar should be set to moveable.
    /// </summary>
    [JsonIgnore] public bool IsMovingEnabled = false;

    //
    // Helper Functions
    //

    /// <summary>
    /// Returns true if conditions are met according to configuration.
    /// </summary>
    public bool ShouldShowHotbar() {
        if (RequireAllConditions) {
            return ShowConditions.TrueForAll(entry => entry.IsConditionMet());
        }
        else {
            return ShowConditions.Exists(entry => entry.IsConditionMet());
        }
    }
}
