using System.Collections.Immutable;

namespace PlainCEETimer.SourceGenerators.Models;

public record PropertyBackingFieldModel(
    string Namespace,
    ImmutableArray<TypeLevel> TypeLevels,
    string GroupKey,
    string PropertyModifiers,
    string PropertyType,
    string PropertyName,
    string BackingFieldName,
    bool IsPropertyStatic,
    bool HasGetter,
    bool HasSetter,
    string GetAccessorModifiers,
    string SetAccessorKeyword,
    string SetAccessorModifiers
);
