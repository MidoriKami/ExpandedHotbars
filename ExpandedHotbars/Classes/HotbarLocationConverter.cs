using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExpandedHotbars.Configuration;

namespace ExpandedHotbars.Classes;

// Because json is stupid, it doesn't like my HotbarLocation class being a dictionary key,
// so we have to convert it to "{row}, {column}" to make it not whine.
public class HotbarLocationConverter : JsonConverter<HotbarLocation> {
    public override void WriteAsPropertyName(Utf8JsonWriter writer, HotbarLocation value, JsonSerializerOptions options)
        => writer.WritePropertyName($"{value.Row},{value.Column}");

    public override HotbarLocation ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var stringKey = reader.GetString();
        var parts = stringKey?.Split(',') ?? [];

        if (parts.Length is 2 && int.TryParse(parts[0], out var row) && int.TryParse(parts[1], out var col)) {
            return new HotbarLocation(row, col);
        }

        return new HotbarLocation(0, 0);
    }

    public override void Write(Utf8JsonWriter writer, HotbarLocation value, JsonSerializerOptions options)
        => writer.WriteStringValue($"{value.Row},{value.Column}");

    public override HotbarLocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => ReadAsPropertyName(ref reader, typeToConvert, options);
}
