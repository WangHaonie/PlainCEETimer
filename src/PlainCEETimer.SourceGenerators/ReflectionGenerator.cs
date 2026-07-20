using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PlainCEETimer.SourceGenerators.Base;
using PlainCEETimer.SourceGenerators.Models;

namespace PlainCEETimer.SourceGenerators;

[Generator]
public sealed class ReflectionGenerator : PlainIncrementalGenerator<ReflectionModel>
{
    private const string Internals = nameof(Internals);
    private const string ReflectionClassAttribute = nameof(ReflectionClassAttribute);
    private const string ReflectionMethodAttribute = nameof(ReflectionMethodAttribute);

    protected override bool CanGenerate(SyntaxNode node, CancellationToken token) => node switch
    {
        MethodDeclarationSyntax mds => mds.AttributeLists.Count > 0,
        _ => false
    };

    protected override ReflectionModel? Transform(GeneratorSyntaxContext context, CancellationToken token) => context.Node switch
    {
        MethodDeclarationSyntax mds => GetModelByMethod(ref context, mds),
        _ => null
    };

    protected override void Generate(SourceProductionContext context, ImmutableArray<ReflectionModel?> model)
    {
        throw new NotImplementedException();
    }

    private static ReflectionModel? GetModelByMethod(ref GeneratorSyntaxContext context, MethodDeclarationSyntax mds)
    {
        throw new NotImplementedException();
    }
}
