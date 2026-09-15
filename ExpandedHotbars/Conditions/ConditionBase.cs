using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace ExpandedHotbars.Conditions;

public abstract partial class ConditionBase {

    /// <summary>
    /// Gets the name of the config type.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the label that will be used to display this condition.
    /// </summary>
    public abstract string Label { get; }

    /// <summary>
    /// Gets if this condition is being inverted.
    /// </summary>
    public bool Invert { get; set; }

    /// <summary>
    /// Evaluate this condition.
    /// </summary>
    protected abstract bool EvaluateCondition();

    /// <summary>
    /// Evaluate if this condition is being satisfied.
    /// </summary>
    public bool IsConditionMet()
        => EvaluateCondition() == !Invert;

    /// <summary>
    /// Set to true to enable the "Configure" button.
    /// </summary>
    public virtual bool HasConfiguration
        => false;

    /// <summary>
    /// Draws the configuration values for this option.
    /// </summary>
    public virtual void DrawConfig() {
        ImGui.Text("Contents Not Defined");
    }

    /// <summary>
    /// Gets the size that should be used for the config popup.
    /// </summary>
    public virtual Vector2 ConfigSize { get; }
        = new Vector2(400.0f, 400.0f);

    /// <summary>
    /// Value indicating if this condition is valid.
    /// Allows disabling conditions without breaking configs/polymorphism.
    /// </summary>
    public virtual bool IsValid
        => true;
}
