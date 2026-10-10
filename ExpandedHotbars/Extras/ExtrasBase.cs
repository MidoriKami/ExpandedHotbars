using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace ExpandedHotbars.Extras;

public abstract partial class ExtrasBase {

    /// <summary>
    /// Gets or sets whether this feature is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets the display name for this extras feature.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the label that will be used to display this condition.
    /// </summary>
    public abstract string Label { get; }

    /// <summary>
    /// Set to true to enable the "Configure" button.
    /// </summary>
    public virtual bool HasConfiguration
        => false;

    /// <summary>
    /// Draws the configuration element for this feature.
    /// </summary>
    public virtual void DrawConfig() {
        ImGui.Text("Contents Not Defined");
    }

    /// <summary>
    /// Gets the size that should be used for the config popup.
    /// </summary>
    public virtual Vector2 ConfigSize { get; }
        = new(400.0f, 400.0f);

    /// <summary>
    /// Invoke this method to process this feature.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Function is invoked when <see cref="Enabled"/> is changed to true.
    /// </summary>
    public virtual void OnEnable() { }

    /// <summary>
    /// Function is invoked when <see cref="Enabled"/> is changed to false.
    /// </summary>
    public virtual void OnDisable() { }

    /// <summary>
    /// Value indicating if this feature is valid.
    /// Allows disabling feature without breaking configs/polymorphism.
    /// </summary>
    public virtual bool IsValid
        => true;
}
