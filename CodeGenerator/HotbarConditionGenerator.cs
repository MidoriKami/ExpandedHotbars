using Microsoft.CodeAnalysis;

namespace CodeGenerator;

[Generator]
public class HotbarConditionGenerator : JsonMappingGenerator {
    protected override string TargetType => "ConditionBase";
    protected override string BaseNameSpace => "ExpandedHotbars.Conditions";
}
