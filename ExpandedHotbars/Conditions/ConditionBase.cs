namespace ExpandedHotbars.Conditions;

public abstract partial class ConditionBase {

    /// <summary>
    /// Gets the label that will be used to display this condition.
    /// </summary>
    public abstract string Label { get; }

    /// <summary>
    /// Evaluate if this condition is being satisfied.
    /// </summary>
    /// <returns></returns>
    public abstract bool IsConditionMet();

    /// <summary>
    /// Draws the configuration values for this option.
    /// </summary>
    public abstract void DrawConfig();

    /// <summary>
    /// Value indicating if this condition is valid.
    /// Allows disabling conditions without breaking configs/polymorphism.
    /// </summary>
    public virtual bool IsValid
        => true;
}
