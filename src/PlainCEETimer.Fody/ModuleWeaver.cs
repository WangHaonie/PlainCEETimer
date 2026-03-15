using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

namespace PlainCEETimer.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    public override bool ShouldCleanReference => true;

    private const string attribName = "PlainCEETimer.Modules.Fody.NoConstantsAttribute";

    public override void Execute()
    {
        var typesWithAttribute = ModuleDefinition.GetTypes()
            .Select(t => new
            {
                Type = t,
                Attr = t.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == attribName)
            })

            .Where(x => x.Attr != null)
            .ToList();

        foreach (var item in typesWithAttribute)
        {
            var typeDef = item.Type;
            var attribute = item.Attr;
            RemoveConstantFields(typeDef);
            typeDef.CustomAttributes.Remove(attribute);
        }

        var attrself = ModuleDefinition.Types.FirstOrDefault(x => x.FullName == attribName);

        if (attrself != null)
        {
            ModuleDefinition.Types.Remove(attrself);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
    }

    private void RemoveConstantFields(TypeDefinition type)
    {
        var constants = type.Fields.Where(f => f.IsLiteral).ToList();

        foreach (var constant in constants)
        {
            type.Fields.Remove(constant);
        }
    }
}