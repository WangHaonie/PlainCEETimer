using System.Text;

namespace PlainCEETimer.SourceGenerators.Utils;

internal sealed class SourceWriter(StringBuilder sb)
{
    private const int IndentSize = 4;

    private int indent;

    public void WriteLine(string? text = null)
    {
        if (!string.IsNullOrEmpty(text))
        {
            sb.Append(' ', indent * IndentSize).Append(text);
        }

        sb.AppendLine();
    }

    public void OpenBlock(string header)
    {
        WriteLine(header);
        WriteLine("{");
        indent++;
    }

    public void CloseBlock()
    {
        indent--;
        WriteLine("}");
    }

    public void CloseBlocks(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CloseBlock();
        }
    }
}
