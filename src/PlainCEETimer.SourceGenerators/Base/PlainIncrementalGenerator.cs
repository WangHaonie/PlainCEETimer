using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace PlainCEETimer.SourceGenerators.Base;

public abstract class PlainIncrementalGenerator<T> : IIncrementalGenerator
{
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context.SyntaxProvider
            .CreateSyntaxProvider(CanGenerate, Transform)
            .Where(m => m != null);

        context.RegisterSourceOutput(source.Collect(), Generate);
    }

    protected abstract bool CanGenerate(SyntaxNode node, CancellationToken token);

    protected abstract T? Transform(GeneratorSyntaxContext context, CancellationToken token);

    protected abstract void Generate(SourceProductionContext context, ImmutableArray<T?> model);
}