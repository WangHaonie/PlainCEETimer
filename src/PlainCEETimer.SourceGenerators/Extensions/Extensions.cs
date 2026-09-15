using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlainCEETimer.SourceGenerators.Extensions;

public static class Extensions
{
    public static bool IsPartial(this INamedTypeSymbol symbol)
    {
        return symbol.DeclaringSyntaxReferences.Any(r =>
            r.GetSyntax() is TypeDeclarationSyntax tds
                && tds.Modifiers.Any(m => m.ValueText == "partial"));
    }
}
