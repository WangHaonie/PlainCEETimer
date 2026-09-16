using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PlainCEETimer.SourceGenerators.Base;
using PlainCEETimer.SourceGenerators.Extensions;
using PlainCEETimer.SourceGenerators.Models;

namespace PlainCEETimer.SourceGenerators;

[Generator]
public sealed class PropertyBackingFieldGenerator : PlainSourceGenerator<PropertyBackingFieldModel>
{
    private const string SGenNamespace = "PlainCEETimer.Modules.Annotations.SourceGenerators";
    private const string BackingFieldAttribute = $"{SGenNamespace}.{nameof(BackingFieldAttribute)}";

    private const string Indent = "    ";
    private const string Indent2 = Indent + Indent;
    private const string Indent3 = Indent2 + Indent;

    private static readonly string CGADecl;
    private static readonly string GCADecl;

    static PropertyBackingFieldGenerator()
    {
        var t = typeof(PropertyBackingFieldGenerator);
        var sgName = t.FullName;
        t = typeof(CompilerGeneratedAttribute);
        var cgaFullName = t.FullName.Replace(nameof(Attribute), string.Empty);
        t = typeof(GeneratedCodeAttribute);
        var gcaFullName = t.FullName.Replace(nameof(Attribute), string.Empty);
        var sgVer = "main";

        CGADecl = $"[global::{cgaFullName}]";
        GCADecl = $"[global::{gcaFullName}(\"{sgName}\", \"{sgVer}\")]";
    }

    protected override bool CanGenerate(SyntaxNode node, CancellationToken token)
    {
        return node is PropertyDeclarationSyntax prop && prop.AttributeLists.Count > 0;
    }

    protected override PropertyBackingFieldModel? Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        var pds = (PropertyDeclarationSyntax)context.Node;

        if (pds.Modifiers.Any(m => m.ValueText == "partial")
            && context.SemanticModel.GetDeclaredSymbol(pds, token) is IPropertySymbol ps)
        {
            var attr = ps.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == BackingFieldAttribute);

            if (attr != null && attr is { ConstructorArguments: var args } && args.Length > 0)
            {
                var name = args[0].Value?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(name))
                {
                    var c = ps.ContainingType;

                    if (c != null && c.IsPartial())
                    {
                        var ns = c.ContainingNamespace.IsGlobalNamespace
                            ? string.Empty
                            : c.ContainingNamespace.ToDisplayString();

                        var types = new List<string>();
                        var typeKindKeywords = new List<string>();
                        var current = c;

                        while (current != null && current.TypeKind != TypeKind.Error)
                        {
                            types.Insert(0, current.Name);
                            typeKindKeywords.Insert(0, GetTypeKindKeyword(current));
                            current = current.ContainingType;
                        }

                        var groupKey = string.IsNullOrEmpty(ns) ? types[0] : $"{ns}.{types[0]}";

                        var hasGetter = false;
                        var hasSetter = false;
                        var getModifiers = string.Empty;
                        var setKeyword = string.Empty;
                        var setModifiers = string.Empty;

                        if (pds.AccessorList != null)
                        {
                            foreach (var acc in pds.AccessorList.Accessors)
                            {
                                switch (acc.Kind())
                                {
                                    case SyntaxKind.GetAccessorDeclaration:
                                        hasGetter = true;
                                        getModifiers = GetAccessorModifiers(acc);
                                        break;
                                    case SyntaxKind.SetAccessorDeclaration:
                                        hasSetter = true;
                                        setKeyword = "set";
                                        setModifiers = GetAccessorModifiers(acc);
                                        break;
                                    case SyntaxKind.InitAccessorDeclaration:
                                        hasSetter = true;
                                        setKeyword = "init";
                                        setModifiers = GetAccessorModifiers(acc);
                                        break;
                                }
                            }
                        }

                        if (hasGetter || hasSetter)
                        {
                            return new(ns, types, typeKindKeywords, groupKey, pds.Modifiers.ToString(), ps.Type.ToDisplayString(),
                                ps.Name, name, ps.IsStatic, hasGetter, hasSetter, getModifiers, setKeyword, setModifiers);
                        }
                    }
                }
            }
        }

        return null;
    }

    protected override void Generate(SourceProductionContext context, ImmutableArray<PropertyBackingFieldModel?> model)
    {
        var models = model.Where(m => m != null).Select(m => m!).ToImmutableArray();

        if (!models.IsEmpty)
        {
            foreach (var group in models.GroupBy(m => m.GroupKey, m => m))
            {
                var first = group.First();
                var thCount = first.TypeHierarchy.Count;

                var sb = new StringBuilder(2048);
                sb.AppendLine("// <auto-generated/>");
                sb.AppendLine("using System;");

                if (!string.IsNullOrEmpty(first.Namespace))
                {
                    sb.AppendLine();
                    sb.AppendLine($"namespace {first.Namespace}");
                    sb.AppendLine("{");
                }

                for (int i = 0; i < thCount; i++)
                {
                    sb.AppendLine($"{Indent}{first.TypeKindKeywords[i]} {first.TypeHierarchy[i]}");
                    sb.AppendLine($"{Indent}{{");
                }

                var propList = group.ToList();

                for (int i = 0; i < propList.Count; i++)
                {
                    var m = propList[i];

                    var fieldStatic = m.IsPropertyStatic ? "static " : string.Empty;
                    var fieldAccess = m.IsPropertyStatic ? m.BackingFieldName : $"this.{m.BackingFieldName}";

                    sb.AppendLine($"{Indent2}{CGADecl}");
                    sb.AppendLine($"{Indent2}private {fieldStatic}{m.PropertyType} {m.BackingFieldName};");
                    sb.AppendLine();
                    sb.AppendLine($"{Indent2}{GCADecl}");
                    sb.AppendLine($"{Indent2}{m.PropertyModifiers} {m.PropertyType} {m.PropertyName}");
                    sb.AppendLine($"{Indent2}{{");

                    if (m.HasGetter)
                        sb.AppendLine($"{Indent3}{m.GetAccessorModifiers}get => {fieldAccess};");
                    if (m.HasSetter)
                        sb.AppendLine($"{Indent3}{m.SetAccessorModifiers}{m.SetAccessorKeyword} => {fieldAccess} = value;");

                    sb.AppendLine($"{Indent2}}}");

                    if (i < propList.Count - 1)
                        sb.AppendLine();
                }

                for (int i = 0; i < thCount; i++)
                {
                    sb.AppendLine($"{Indent}}}");
                }

                if (!string.IsNullOrEmpty(first.Namespace))
                {
                    sb.AppendLine("}");
                }

                context.AddSource(first.GroupKey + ".g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            }
        }
    }

    private static string GetTypeKindKeyword(INamedTypeSymbol symbol)
    {
        if (symbol.IsRecord)
            return symbol.TypeKind == TypeKind.Struct ? "partial record struct" : "partial record";

        if (symbol.TypeKind == TypeKind.Struct)
            return symbol.IsReadOnly ? "partial readonly struct" : "partial struct";

        return symbol.IsStatic ? "static partial class" : "partial class";
    }

    private static string GetAccessorModifiers(AccessorDeclarationSyntax acc)
    {
        return acc.Modifiers.Count > 0 ? acc.Modifiers.ToString() + " " : string.Empty;
    }
}
