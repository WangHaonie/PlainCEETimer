namespace PlainCEETimer.SourceGenerators.Models;

public record PropertyBackingFieldModel(
    string Namespace,
    List<string> TypeHierarchy,
    string GroupKey,
    string PropertyModifiers,
    string PropertyType,
    string PropertyName,
    string BackingFieldName
);
