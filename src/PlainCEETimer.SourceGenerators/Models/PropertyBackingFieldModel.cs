﻿namespace PlainCEETimer.SourceGenerators.Models;

public record PropertyBackingFieldModel(
    string Namespace,
    List<string> TypeHierarchy,
    List<string> TypeKindKeywords,
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
