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
        Handle_NoConstantsAttribute(new("NoConstantsAttribute", "PlainCEETimer.Modules.Fody.NoConstantsAttribute"));
        Handle_CompilerRemoveAttribute(new("CompilerRemoveAttribute", "PlainCEETimer.Modules.Fody.CompilerRemoveAttribute"));
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
    }

    private void Handle_NoConstantsAttribute(TypeBasicInfo info)
    {
        var types = GetTypeFromAttribute(info, t => t.IsClass || t.IsStruct, out var length);

        for (int i = 0; i < length; i++)
        {
            var current = types[i];
            var fields = current.Type.Fields;
            var consts = fields.Where(f => f.IsLiteral).ToList();
            var count = consts.Count;

            for (int j = 0; j < count; j++)
            {
                fields.Remove(consts[j]);
            }

            RemoveAttribute(current);
        }

        DeleteType(info);
    }

    private void Handle_CompilerRemoveAttribute(TypeBasicInfo info)
    {
        var types = GetTypeFromAttribute(info, t => t.IsClass, out var count);

        for (int i = 0; i < count; i++)
        {
            DeleteType(types[i].Type);
        }

        DeleteType(info);
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

    private void DeleteType(TypeBasicInfo info)
    {
        DeleteType(ModuleDefinition.Types.FirstOrDefault(x => x.Name == info.Name && x.FullName == info.FullName));
    }

    private void DeleteType(TypeDefinition type)
    {
        if (type != null)
        {
            ModuleDefinition.Types.Remove(type);
        }
    }
}