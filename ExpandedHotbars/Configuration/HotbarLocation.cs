using System.Text.Json.Serialization;
using ExpandedHotbars.Classes;

namespace ExpandedHotbars.Configuration;

[JsonConverter(typeof(HotbarLocationConverter))]
public record HotbarLocation(int Row, int Column) {
    public static implicit operator HotbarLocation((int Row, int Column) tuple)
        => new(tuple.Row, tuple.Column);
}
