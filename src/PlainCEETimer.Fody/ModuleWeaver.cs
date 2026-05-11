using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Fody;
using Mono.Cecil;

namespace PlainCEETimer.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    private sealed class TypeAndAttribute(TypeDefinition type, CustomAttribute attribute)
    {
        public TypeDefinition Type => type;

        public CustomAttribute Attribute => attribute;
    }

    private sealed class AttributeInfo(string ns, string name)
    {
        public string Name => name;

        public string Namespace => ns;
    }

    private const string BaseNamespace = "PlainCEETimer.Modules.Fody";

    private static readonly AttributeInfo NoConstantsAttribute = new(BaseNamespace, nameof(NoConstantsAttribute));
    private static readonly AttributeInfo ConstantAttribute = new(BaseNamespace, nameof(ConstantAttribute));
    private static readonly AttributeInfo CompilerRemoveAttribute = new(BaseNamespace, nameof(CompilerRemoveAttribute));

    public override bool ShouldCleanReference => true;

    public override void Execute()
    {
        HandleNoConstantsAttribute();
        HandleCompilerRemoveAttribute();
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
    }

    private void HandleNoConstantsAttribute()
    {
        var types = GetTypesWithAttribute(NoConstantsAttribute, t => t.IsClass || t.IsStruct);

        foreach (var type in types)
        {
            RemoveUnmarkedConstants(type.Type);
            type.Type.CustomAttributes.Remove(type.Attribute);
        }

        TryRemoveType(NoConstantsAttribute);
        TryRemoveType(ConstantAttribute);
    }

    private void HandleCompilerRemoveAttribute()
    {
        var types = GetTypesWithAttribute(CompilerRemoveAttribute, t => t.IsClass);

        foreach (var type in types)
        {
            TryRemoveType(type.Type);
        }

        TryRemoveType(CompilerRemoveAttribute);
    }

    private ReadOnlyCollection<TypeAndAttribute> GetTypesWithAttribute(AttributeInfo info, Predicate<TypeDefinition> isAttributeTarget)
    {
        return ModuleDefinition.GetTypes()
            .Where(t => isAttributeTarget(t))
            .Select(t => new TypeAndAttribute(t, FindAttribute(t, info)))
            .Where(x => x.Attribute != null).ToList().AsReadOnly();
    }

    private void RemoveUnmarkedConstants(TypeDefinition type)
    {
        foreach (var field in type.Fields.Where(x => x.IsLiteral).ToList())
        {
            var a = FindAttribute(field, ConstantAttribute);

            if (a != null)
            {
                field.CustomAttributes.Remove(a);
                continue;
            }

            type.Fields.Remove(field);
        }
    }

    private bool TryRemoveType(AttributeInfo info)
    {
        return TryRemoveType(ModuleDefinition.GetTypes()
            .FirstOrDefault(t => t.Name == info.Name && t.Namespace == info.Namespace));
    }

    private static CustomAttribute FindAttribute(ICustomAttributeProvider provider, AttributeInfo info)
    {
        if (!provider.HasCustomAttributes)
        {
            return null;
        }

        return provider.CustomAttributes
            .FirstOrDefault(x => x.AttributeType.Name == info.Name && x.AttributeType.Namespace == info.Namespace);
    }

    private static bool TryRemoveType(TypeDefinition type)
    {
        if (type == null)
        {
            return false;
        }

        if (type.IsNested)
        {
            return type.DeclaringType.NestedTypes.Remove(type);
        }

        return type.Module.Types.Remove(type);
    }
}
