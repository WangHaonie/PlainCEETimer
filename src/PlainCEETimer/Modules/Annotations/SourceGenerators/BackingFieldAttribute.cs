using System;

namespace PlainCEETimer.Modules.Annotations.SourceGenerators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class BackingFieldAttribute(string fieldName) : Attribute
{
    public string FieldName => fieldName;
}
