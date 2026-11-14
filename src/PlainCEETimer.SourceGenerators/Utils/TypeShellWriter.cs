using System.Collections.Immutable;
using PlainCEETimer.SourceGenerators.Models;

namespace PlainCEETimer.SourceGenerators.Utils;

internal static class TypeShellWriter
{
    public static int OpenTypeShell(this SourceWriter writer, string ns, ImmutableArray<TypeLevel> levels)
    {
        var blocks = 0;

        if (!string.IsNullOrEmpty(ns))
        {
            writer.OpenBlock($"namespace {ns}");
            blocks++;
        }

        foreach (var level in levels)
        {
            writer.OpenBlock($"{level.Keyword} {level.Name}");
            blocks++;
        }

        return blocks;
    }
}
