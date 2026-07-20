namespace PlainCEETimer.SourceGenerators.Models;

public record ReflectionModel(
    string Namespace,
    string ClassName,
    string ReflectClassName,

    bool IsInternals,
    bool IsPartial,
    bool IsStatic
);