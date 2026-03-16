using System;
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

namespace PlainCEETimer.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    private class TypeAndAttribute(TypeDefinition type, CustomAttribute attribute)
    {
        public TypeDefinition Type => type;

        public CustomAttribute Attribute => attribute;
    }

    private class TypeBasicInfo(string name, string fullName)
    {
        public string Name => name;

        public string FullName => fullName;
    }

    public override bool ShouldCleanReference => true;

    public override void Execute()
    {
        Handle_NoConstantsAttribute(new("NoConstantsAttribute", "PlainCEETimer.Modules.Fody.NoConstantsAttribute"),
            new("ConstantAttribute", "PlainCEETimer.Modules.Fody.ConstantAttribute"));
        Handle_CompilerRemoveAttribute(new("CompilerRemoveAttribute", "PlainCEETimer.Modules.Fody.CompilerRemoveAttribute"));
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
    }

    private void Handle_NoConstantsAttribute(TypeBasicInfo mainInfo, TypeBasicInfo excludeFlag)
    {
        var types = GetTypeFromAttribute(mainInfo, t => t.IsClass || t.IsStruct, out var length);

        for (int i = 0; i < length; i++)
        {
            var current = types[i];
            var fields = current.Type.Fields;

            var consts = fields.Where(f =>
            {
                if (f.IsLiteral)
                {
                    if (f.HasCustomAttributes)
                    {
                        if (f.CustomAttributes.Any(a =>
                        {
                            var at = a.AttributeType;

                            if (at.Name == excludeFlag.Name && at.FullName == excludeFlag.FullName)
                            {
                                f.CustomAttributes.Remove(a);
                                return true;
                            }

                            return false;
                        }))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }).ToList();

            var count = consts.Count;

            for (int j = 0; j < count; j++)
            {
                fields.Remove(consts[j]);
            }

            RemoveAttribute(current);
        }

        TryDeleteType(mainInfo);
        TryDeleteType(excludeFlag);
    }

    private void Handle_CompilerRemoveAttribute(TypeBasicInfo info)
    {
        var types = GetTypeFromAttribute(info, t => t.IsClass, out var count);

        for (int i = 0; i < count; i++)
        {
            TryDeleteType(types[i].Type);
        }

        TryDeleteType(info);
    }

    private List<TypeAndAttribute> GetTypeFromAttribute(TypeBasicInfo info, Predicate<TypeDefinition> IsAttributeTarget, out int count)
    {
        List<TypeAndAttribute> result = null;

        try
        {
            result = [.. ModuleDefinition.GetTypes()
                .Where(t => IsAttributeTarget(t) && t.HasCustomAttributes
                    && t.CustomAttributes.Any(a => a.AttributeType.Name == info.Name))
                .Select(x => new TypeAndAttribute(x, x.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == info.FullName)))
                .Where(o => o.Attribute != null)];
        }
        catch (Exception ex)
        {
            if (ex is not NullReferenceException and not ArgumentNullException)
            {
                throw;
            }
        }

        count = result?.Count ?? 0;
        return result;
    }

    private void RemoveAttribute(TypeAndAttribute typeAndAttribute)
    {
        typeAndAttribute.Type.CustomAttributes.Remove(typeAndAttribute.Attribute);
    }

    private bool TryDeleteType(TypeBasicInfo info)
    {
        return TryDeleteType(ModuleDefinition.Types.FirstOrDefault(x => x.Name == info.Name && x.FullName == info.FullName));
    }

    private bool TryDeleteType(TypeDefinition type)
    {
        try
        {
            if (type != null)
            {
                if (type.IsNested)
                {
                    type.DeclaringType.NestedTypes.Remove(type);
                }

                ModuleDefinition.Types.Remove(type);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}