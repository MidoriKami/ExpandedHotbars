namespace ExpandedHotbars.Extras;

public abstract partial class ExtrasBase {

    /// <summary>
    /// Gets the display name for this extras feature.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets an optional help message for this feature.
    /// </summary>
    public virtual string? HelpMessage
        => null;

}
