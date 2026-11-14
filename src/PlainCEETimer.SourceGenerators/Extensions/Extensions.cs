using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PlainCEETimer.SourceGenerators.Models;

namespace PlainCEETimer.SourceGenerators.Extensions;

public static class Extensions
{
    public static bool IsPartial(this INamedTypeSymbol symbol)
    {
        return symbol.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is TypeDeclarationSyntax tds && tds.IsPartialDefined());
    }

    public static bool IsPartialDefined(this MemberDeclarationSyntax mds)
    {
        return mds.Modifiers.Any(m => m.ValueText == "partial");
    }

    public static AttributeData? GetAttribute(this ISymbol symbol, string fullName)
    {
        return symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == fullName);
    }

    public static ImmutableArray<TypeLevel> GetTypeLevels(this INamedTypeSymbol symbol)
    {
        var levels = ImmutableArray.CreateBuilder<TypeLevel>();

        for (var current = symbol; current != null; current = current.ContainingType)
        {
            if (current.TypeKind == TypeKind.Error || !current.IsPartial())
            {
                return [];
            }

            levels.Insert(0, new TypeLevel(GetTypeKeyword(current), GetTypeName(current)));
        }

        return levels.ToImmutable();
    }

    public static string GetTypePath(this INamedTypeSymbol symbol)
    {
        var parts = new List<string>();

        for (var current = symbol; current != null; current = current.ContainingType)
        {
            parts.Insert(0, current.MetadataName);
        }

        return string.Join(".", parts);
    }

    private static string GetTypeKeyword(INamedTypeSymbol symbol)
    {
        return symbol.TypeKind switch
        {
            TypeKind.Interface => "partial interface",
            TypeKind.Struct when symbol.IsRecord => symbol.IsReadOnly ? "readonly partial record struct" : "partial record struct",
            TypeKind.Struct => BuildStructKeyword(symbol),
            _ => symbol.IsRecord ? "partial record" : GetClassKeyword(symbol),
        };
    }

    private static string BuildStructKeyword(INamedTypeSymbol symbol)
    {
        var sb = new StringBuilder();

        if (symbol.IsReadOnly)
        {
            sb.Append("readonly ");
        }

        if (symbol.IsRefLikeType)
        {
            sb.Append("ref ");
        }

        return sb.Append("partial struct").ToString();
    }

    private static string GetClassKeyword(INamedTypeSymbol symbol)
    {
        return symbol.IsStatic ? "static partial class" : "partial class";
    }

    private static string GetTypeName(INamedTypeSymbol symbol)
    {
        if (symbol.TypeParameters.Length == 0)
        {
            return symbol.Name;
        }

        return $"{symbol.Name}<{string.Join(", ", symbol.TypeParameters.Select(t => t.Name))}>";
    }
}
