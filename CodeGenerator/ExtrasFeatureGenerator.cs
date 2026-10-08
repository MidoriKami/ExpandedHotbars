using Microsoft.CodeAnalysis;

namespace CodeGenerator;

[Generator]
public class ExtrasFeatureGenerator : JsonMappingGenerator {
    protected override string TargetType => "ExtrasBase";
    protected override string BaseNameSpace => "ExpandedHotbars.Extras";
}
